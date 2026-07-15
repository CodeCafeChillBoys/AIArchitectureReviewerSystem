using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace AIArchitectureReviewer.Application.Orchestrators
{
    public partial class UMLReviewOrchestrator
    {
        private static readonly Regex InvalidCharsRegex = new Regex(@"[@#\$%\^&!~|\\?`]", RegexOptions.Compiled);

        private (float score, string details, JsonNode scoreJson) CalculateScoreProgrammatically(JsonNode? parsedDiagram)
        {
            float score = 10.0f;
            var detailsList = new List<string>();

            if (parsedDiagram == null)
            {
                return (10.0f, "Không thể phân tích sơ đồ JSON để tính điểm. Trả về điểm mặc định.", new JsonObject
                {
                    ["total_score"] = 10.0f,
                    ["details"] = "Không thể phân tích sơ đồ JSON để tính điểm. Trả về điểm mặc định.",
                    ["level"] = "Excellent"
                });
            }

            var diagramType = parsedDiagram["diagram_type"]?.ToString() ?? "Global";
            var nodes = parsedDiagram["nodes"]?.AsArray();
            var relationships = parsedDiagram["relationships"]?.AsArray();

            // 1. God Class/Service Check
            int godClassCount = 0;
            var godClassDeductions = new List<string>();
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    if (node == null) continue;
                    var name = node["name"]?.ToString() ?? "Unknown";
                    var type = node["type"]?.ToString() ?? "Class";

                    if (type.Equals("Class", StringComparison.OrdinalIgnoreCase) || 
                        type.Equals("Service", StringComparison.OrdinalIgnoreCase))
                    {
                        int attrCount = node["attributes"]?.AsArray()?.Count ?? 0;
                        int methodCount = node["methods"]?.AsArray()?.Count ?? 0;
                        int total = attrCount + methodCount;
                        if (total > 10)
                        {
                            godClassCount++;
                            godClassDeductions.Add($"`{name}` ({total} members)");
                        }
                    }
                }
            }
            float godClassPenalty = godClassCount * 1.5f;

            // 2. High Coupling Check
            int highCoupledCount = 0;
            var couplingDeductions = new List<string>();
            var relationshipCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            if (relationships != null)
            {
                foreach (var rel in relationships)
                {
                    if (rel == null) continue;
                    var from = rel["from"]?.ToString();
                    var to = rel["to"]?.ToString();
                    if (!string.IsNullOrEmpty(from))
                    {
                        relationshipCounts[from] = relationshipCounts.GetValueOrDefault(from) + 1;
                    }
                    if (!string.IsNullOrEmpty(to))
                    {
                        relationshipCounts[to] = relationshipCounts.GetValueOrDefault(to) + 1;
                    }
                }
            }
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    if (node == null) continue;
                    var name = node["name"]?.ToString();
                    if (string.IsNullOrEmpty(name)) continue;

                    int relCount = relationshipCounts.GetValueOrDefault(name);
                    if (relCount > 5)
                    {
                        highCoupledCount++;
                        couplingDeductions.Add($"`{name}` ({relCount} rels)");
                    }
                }
            }
            float couplingPenalty = highCoupledCount * 1.0f;

            // 3. Cyclic Dependency Check
            bool hasCycle = false;
            var cycleDeductions = new List<string>();
            if (relationships != null)
            {
                var adj = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                foreach (var rel in relationships)
                {
                    if (rel == null) continue;
                    var from = rel["from"]?.ToString();
                    var to = rel["to"]?.ToString();
                    if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to)) continue;

                    if (!adj.ContainsKey(from)) adj[from] = new List<string>();
                    adj[from].Add(to);
                }

                var visited = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase); // 0=unvisited, 1=visiting, 2=visited
                bool Dfs(string u, List<string> path)
                {
                    visited[u] = 1;
                    path.Add(u);

                    if (adj.ContainsKey(u))
                    {
                        foreach (var v in adj[u])
                        {
                            if (visited.GetValueOrDefault(v) == 1)
                            {
                                var cycleStartIdx = path.IndexOf(v);
                                var cyclePath = string.Join(" -> ", path.Skip(cycleStartIdx)) + " -> " + v;
                                cycleDeductions.Add(cyclePath);
                                return true;
                            }
                            else if (visited.GetValueOrDefault(v) == 0)
                            {
                                if (Dfs(v, path)) return true;
                            }
                        }
                    }

                    path.RemoveAt(path.Count - 1);
                    visited[u] = 2;
                    return false;
                }

                foreach (var nodeName in adj.Keys)
                {
                    if (visited.GetValueOrDefault(nodeName) == 0)
                    {
                        if (Dfs(nodeName, new List<string>()))
                        {
                            hasCycle = true;
                            break;
                        }
                    }
                }
            }
            float cyclePenalty = hasCycle ? 2.0f : 0.0f;

            // 4. Empty Class Check
            int emptyClassCount = 0;
            var emptyClassDeductions = new List<string>();
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    if (node == null) continue;
                    var name = node["name"]?.ToString() ?? "Unknown";
                    var type = node["type"]?.ToString() ?? "Class";

                    if (type.Equals("Class", StringComparison.OrdinalIgnoreCase))
                    {
                        int attrCount = node["attributes"]?.AsArray()?.Count ?? 0;
                        int methodCount = node["methods"]?.AsArray()?.Count ?? 0;
                        if (attrCount == 0 && methodCount == 0)
                        {
                            emptyClassCount++;
                            emptyClassDeductions.Add($"`{name}`");
                        }
                    }
                }
            }
            float emptyClassPenalty = emptyClassCount * 0.5f;

            // 5. Self Dependency Check
            int selfDepCount = 0;
            var selfDepDeductions = new List<string>();
            if (relationships != null)
            {
                foreach (var rel in relationships)
                {
                    if (rel == null) continue;
                    var from = rel["from"]?.ToString();
                    var to = rel["to"]?.ToString();
                    if (!string.IsNullOrEmpty(from) && from.Equals(to, StringComparison.OrdinalIgnoreCase))
                    {
                        selfDepCount++;
                        selfDepDeductions.Add($"`{from}`");
                    }
                }
            }
            float selfDepPenalty = selfDepCount * 0.5f;

            // 6. Invalid Characters Check
            int invalidCharCount = 0;
            var invalidCharDeductions = new List<string>();
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    if (node == null) continue;
                    var nodeName = node["name"]?.ToString();
                    if (!string.IsNullOrEmpty(nodeName) && InvalidCharsRegex.IsMatch(nodeName))
                    {
                        invalidCharCount++;
                        invalidCharDeductions.Add($"Lớp `{nodeName}`");
                    }

                    var attrs = node["attributes"]?.AsArray();
                    if (attrs != null)
                    {
                        foreach (var attr in attrs)
                        {
                            var attrStr = attr?.ToString();
                            if (!string.IsNullOrEmpty(attrStr) && InvalidCharsRegex.IsMatch(attrStr))
                            {
                                invalidCharCount++;
                                invalidCharDeductions.Add($"Thuộc tính `{attrStr}` trong `{nodeName}`");
                            }
                        }
                    }

                    var methods = node["methods"]?.AsArray();
                    if (methods != null)
                    {
                        foreach (var method in methods)
                        {
                            var methodStr = method?.ToString();
                            if (!string.IsNullOrEmpty(methodStr) && InvalidCharsRegex.IsMatch(methodStr))
                            {
                                invalidCharCount++;
                                invalidCharDeductions.Add($"Phương thức `{methodStr}` trong `{nodeName}`");
                            }
                        }
                    }
                }
            }
            float invalidCharPenalty = invalidCharCount * 0.5f;

            // 7. Invalid Arrow/Relationship Type Check
            int invalidArrowCount = 0;
            var invalidArrowDeductions = new List<string>();

            var classValid = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "inheritance", "generalization", "realization", "implementation", "association", "dependency", "aggregation", "composition" };
            var sequenceValid = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "call", "return", "send", "receive", "message", "create", "destroy" };
            var usecaseValid = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "association", "extend", "include", "generalization" };
            var componentValid = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "dependency", "realization", "interface", "connector", "association" };
            var flowValid = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "flow", "next", "transition", "arrow", "sequence", "controlflow", "objectflow" };

            HashSet<string>? validSet = null;
            if (diagramType.Contains("class", StringComparison.OrdinalIgnoreCase)) validSet = classValid;
            else if (diagramType.Contains("sequence", StringComparison.OrdinalIgnoreCase)) validSet = sequenceValid;
            else if (diagramType.Contains("usecase", StringComparison.OrdinalIgnoreCase) || diagramType.Contains("use case", StringComparison.OrdinalIgnoreCase)) validSet = usecaseValid;
            else if (diagramType.Contains("component", StringComparison.OrdinalIgnoreCase)) validSet = componentValid;
            else if (diagramType.Contains("flowchart", StringComparison.OrdinalIgnoreCase) || diagramType.Contains("activity", StringComparison.OrdinalIgnoreCase)) validSet = flowValid;

            if (validSet != null && relationships != null)
            {
                foreach (var rel in relationships)
                {
                    if (rel == null) continue;
                    var from = rel["from"]?.ToString() ?? "Unknown";
                    var to = rel["to"]?.ToString() ?? "Unknown";
                    var relType = rel["type"]?.ToString()?.Trim();
                    if (!string.IsNullOrEmpty(relType) && !validSet.Contains(relType))
                    {
                        invalidArrowCount++;
                        invalidArrowDeductions.Add($"`{from} --({relType})--> {to}`");
                    }
                }
            }
            float invalidArrowPenalty = invalidArrowCount * 1.0f;

            // Calculate final score
            score = 10.0f - godClassPenalty - couplingPenalty - cyclePenalty - emptyClassPenalty - selfDepPenalty - invalidCharPenalty - invalidArrowPenalty;
            score = Math.Max(0.0f, score);

            // Classification Level
            string level = "Excellent";
            if (score < 5.0f) level = "Critical";
            else if (score < 7.0f) level = "Needs Improvement";
            else if (score < 8.5f) level = "Good";

            // Formulate details
            if (godClassCount > 0) detailsList.Add($"- God Class: Trừ {godClassPenalty} điểm ({string.Join(", ", godClassDeductions)})");
            if (highCoupledCount > 0) detailsList.Add($"- High Coupling: Trừ {couplingPenalty} điểm ({string.Join(", ", couplingDeductions)})");
            if (hasCycle) detailsList.Add($"- Cyclic Dependency: Trừ {cyclePenalty} điểm ({string.Join(", ", cycleDeductions)})");
            if (emptyClassCount > 0) detailsList.Add($"- Empty Class: Trừ {emptyClassPenalty} điểm ({string.Join(", ", emptyClassDeductions)})");
            if (selfDepCount > 0) detailsList.Add($"- Self Dependency: Trừ {selfDepPenalty} điểm ({string.Join(", ", selfDepDeductions)})");
            if (invalidCharCount > 0) detailsList.Add($"- Invalid Characters: Trừ {invalidCharPenalty} điểm ({string.Join(", ", invalidCharDeductions)})");
            if (invalidArrowCount > 0) detailsList.Add($"- Invalid Arrow/Relationship Type: Trừ {invalidArrowPenalty} điểm ({string.Join(", ", invalidArrowDeductions)})");

            if (detailsList.Count == 0)
            {
                detailsList.Add("Kiến trúc sơ đồ đáp ứng đầy đủ tất cả các quy tắc thiết kế hệ thống.");
            }

            string scoreDetails = string.Join("\n", detailsList);

            var scoreJson = new JsonObject
            {
                ["total_score"] = score,
                ["details"] = scoreDetails,
                ["level"] = level
            };

            return (score, scoreDetails, scoreJson);
        }
    }
}
