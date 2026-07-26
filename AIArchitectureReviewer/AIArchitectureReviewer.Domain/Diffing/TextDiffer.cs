using System;
using System.Collections.Generic;
using System.Text;

namespace AIArchitectureReviewer.Domain.Diffing
{
    public static class TextDiffer
    {
        /// <summary>
        /// Trên ngưỡng này, bảng LCS chiếm quá nhiều bộ nhớ (O(m*n) int).
        /// Vượt ngưỡng thì coi như toàn bộ nội dung bị thay thế.
        /// </summary>
        public const int MaxLinesForLineDiff = 3000;

        private static readonly string[] EmptyLines = Array.Empty<string>();

        /// <summary>Chuẩn hoá xuống dòng về LF. Bắt buộc gọi trước khi so sánh text.</summary>
        public static string Normalize(string? text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Replace("\r\n", "\n").Replace('\r', '\n');
        }

        public static IReadOnlyList<DiffLine> Diff(string? oldText, string? newText)
        {
            var oldLines = SplitLines(oldText);
            var newLines = SplitLines(newText);

            if (oldLines.Length > MaxLinesForLineDiff || newLines.Length > MaxLinesForLineDiff)
                return WholeReplacement(oldLines, newLines);

            int m = oldLines.Length;
            int n = newLines.Length;

            // dp[i, j] = độ dài dãy con chung dài nhất của oldLines[i..] và newLines[j..]
            var dp = new int[m + 1, n + 1];
            for (int i = m - 1; i >= 0; i--)
            {
                for (int j = n - 1; j >= 0; j--)
                {
                    dp[i, j] = string.Equals(oldLines[i], newLines[j], StringComparison.Ordinal)
                        ? dp[i + 1, j + 1] + 1
                        : Math.Max(dp[i + 1, j], dp[i, j + 1]);
                }
            }

            var result = new List<DiffLine>();
            int x = 0, y = 0;
            while (x < m && y < n)
            {
                if (string.Equals(oldLines[x], newLines[y], StringComparison.Ordinal))
                {
                    result.Add(new DiffLine(DiffOp.Equal, oldLines[x], x + 1, y + 1));
                    x++;
                    y++;
                }
                else if (dp[x + 1, y] >= dp[x, y + 1])
                {
                    result.Add(new DiffLine(DiffOp.Delete, oldLines[x], x + 1, null));
                    x++;
                }
                else
                {
                    result.Add(new DiffLine(DiffOp.Insert, newLines[y], null, y + 1));
                    y++;
                }
            }
            while (x < m)
            {
                result.Add(new DiffLine(DiffOp.Delete, oldLines[x], x + 1, null));
                x++;
            }
            while (y < n)
            {
                result.Add(new DiffLine(DiffOp.Insert, newLines[y], null, y + 1));
                y++;
            }

            return result;
        }

        private static IReadOnlyList<DiffLine> WholeReplacement(string[] oldLines, string[] newLines)
        {
            var result = new List<DiffLine>(oldLines.Length + newLines.Length);
            for (int i = 0; i < oldLines.Length; i++)
                result.Add(new DiffLine(DiffOp.Delete, oldLines[i], i + 1, null));
            for (int j = 0; j < newLines.Length; j++)
                result.Add(new DiffLine(DiffOp.Insert, newLines[j], null, j + 1));
            return result;
        }

        internal static string[] SplitLines(string? text)
        {
            var normalized = Normalize(text);
            if (normalized.Length == 0) return EmptyLines;

            var lines = normalized.Split('\n');

            // "a\n" là một dòng "a", không phải hai dòng "a" và "".
            if (normalized.EndsWith('\n'))
                Array.Resize(ref lines, lines.Length - 1);

            return lines;
        }

        public static IReadOnlyList<DiffHunk> ToHunks(IReadOnlyList<DiffLine> lines, int context = 3)
        {
            var changedIndexes = new List<int>();
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Op != DiffOp.Equal) changedIndexes.Add(i);
            }

            if (changedIndexes.Count == 0) return Array.Empty<DiffHunk>();

            // Gom các vị trí thay đổi thành khoảng [start, end] có kèm ngữ cảnh.
            // Hai khoảng chạm hoặc chồng nhau thì nhập làm một.
            var ranges = new List<(int Start, int End)>();
            int rangeStart = Math.Max(0, changedIndexes[0] - context);
            int rangeEnd = Math.Min(lines.Count - 1, changedIndexes[0] + context);

            for (int k = 1; k < changedIndexes.Count; k++)
            {
                int start = Math.Max(0, changedIndexes[k] - context);
                int end = Math.Min(lines.Count - 1, changedIndexes[k] + context);

                if (start <= rangeEnd + 1)
                {
                    rangeEnd = end;
                }
                else
                {
                    ranges.Add((rangeStart, rangeEnd));
                    rangeStart = start;
                    rangeEnd = end;
                }
            }
            ranges.Add((rangeStart, rangeEnd));

            var hunks = new List<DiffHunk>(ranges.Count);
            foreach (var (start, end) in ranges)
            {
                var slice = new List<DiffLine>(end - start + 1);
                int oldStart = 0, newStart = 0, oldCount = 0, newCount = 0;

                for (int i = start; i <= end; i++)
                {
                    var line = lines[i];
                    slice.Add(line);

                    if (line.Op != DiffOp.Insert)
                    {
                        if (oldCount == 0) oldStart = line.OldLineNo!.Value;
                        oldCount++;
                    }
                    if (line.Op != DiffOp.Delete)
                    {
                        if (newCount == 0) newStart = line.NewLineNo!.Value;
                        newCount++;
                    }
                }

                hunks.Add(new DiffHunk(oldStart, oldCount, newStart, newCount, slice));
            }

            return hunks;
        }

        public static string ToUnifiedDiff(IReadOnlyList<DiffHunk> hunks)
        {
            if (hunks.Count == 0) return string.Empty;

            var sb = new StringBuilder();
            foreach (var hunk in hunks)
            {
                sb.Append("@@ -").Append(hunk.OldStart).Append(',').Append(hunk.OldCount)
                  .Append(" +").Append(hunk.NewStart).Append(',').Append(hunk.NewCount)
                  .Append(" @@\n");

                foreach (var line in hunk.Lines)
                {
                    char prefix = line.Op switch
                    {
                        DiffOp.Insert => '+',
                        DiffOp.Delete => '-',
                        _ => ' '
                    };
                    sb.Append(prefix).Append(line.Text).Append('\n');
                }
            }
            return sb.ToString();
        }
    }
}
