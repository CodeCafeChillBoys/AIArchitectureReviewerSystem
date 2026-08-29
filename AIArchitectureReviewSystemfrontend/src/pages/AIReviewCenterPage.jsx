import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  ArrowLeft,
  Bot,
  Award,
  FileCheck2,
  Code2,
  MessageSquare,
  Loader2,
  AlertCircle,
  Copy,
  Check,
  Edit3,
} from 'lucide-react';
import { diagramService } from '../services/diagramService';
import { reviewService } from '../services/reviewService';
import MermaidRenderer from '../components/common/MermaidRenderer';
import AIChatPanel from '../components/review/AIChatPanel';
import MarkdownReportViewer from '../components/review/MarkdownReportViewer';

export default function AIReviewCenterPage() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [diagram, setDiagram] = useState(null);
  const [report, setReport] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [activeTab, setActiveTab] = useState('report'); // 'report' | 'refactor' | 'chat'
  const [copied, setCopied] = useState(false);

  useEffect(() => {
    const fetchDiagramAndReport = async () => {
      if (!id) return;
      try {
        setLoading(true);
        setError(null);

        // 1. Fetch Diagram info
        const diagramRes = await diagramService.getDiagramById(id);
        const diagramData = diagramRes?.data?.data || diagramRes?.data || diagramRes;
        setDiagram(diagramData);

        // 2. Fetch Review Report by currentVersionId
        const versionId = diagramData?.currentVersionId || diagramData?.id;
        if (versionId) {
          const reportRes = await reviewService.getReportByVersionId(versionId);
          setReport(reportRes?.data?.data || reportRes?.data || reportRes);
        }
      } catch (err) {
        console.error('Failed to load review report:', err);
        setError(
          err.response?.data?.message ||
          err.message ||
          'Failed to load AI review report for this diagram.'
        );
      } finally {
        setLoading(false);
      }
    };

    fetchDiagramAndReport();
  }, [id]);

  // Extract analysis fields
  const rawScore = report?.totalScore ?? 8.5;
  const scoreOutOf10 = rawScore > 10 ? +(rawScore / 10).toFixed(1) : +(Number(rawScore).toFixed(1));
  const reviewData = report?.review || {};
  const reviewDetails = reviewData.ReviewDetails || reviewData.reviewDetails || (typeof reviewData === 'string' ? reviewData : '');
  const refactoredMermaid = reviewData.RefactoredMermaid || reviewData.refactoredMermaid || '';

  const handleCopyCode = () => {
    if (!refactoredMermaid) return;
    navigator.clipboard.writeText(refactoredMermaid);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  const getScoreColor = (score) => {
    if (score >= 8.0) return 'var(--color-success)';
    if (score >= 5.0) return 'var(--color-warning)';
    return 'var(--color-danger)';
  };

  return (
    <div style={{ padding: '24px 28px', maxWidth: '1600px', margin: '0 auto' }}>
      {/* Top Header Row */}
      <div style={{
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        flexWrap: 'wrap',
        gap: '16px',
        marginBottom: '24px',
      }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '14px' }}>
          <button
            onClick={() => diagram?.workspaceId ? navigate(`/workspace/${diagram.workspaceId}`) : navigate('/dashboard')}
            className="btn btn-outline btn-sm"
            style={{ padding: '8px 12px', gap: '6px', fontSize: '13px' }}
          >
            <ArrowLeft size={16} />
            <span>Back to Workspace</span>
          </button>

          <div>
            <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
              <h1 style={{ fontSize: '20px', fontWeight: 700, color: 'var(--text-primary)' }}>
                {diagram?.name || 'AI Review Report'}
              </h1>
              <span className="badge badge-success">Completed</span>
              <span style={{
                fontSize: '11px',
                fontWeight: 600,
                padding: '2px 8px',
                borderRadius: '10px',
                backgroundColor: 'var(--accent-blue-light)',
                color: 'var(--accent-primary)',
              }}>
                v{diagram?.currentVersion || 1}
              </span>
            </div>
            <p style={{ fontSize: '12.5px', color: 'var(--text-muted)', marginTop: '2px' }}>
              Diagram Type: <strong>{diagram?.diagramType || 'Diagram'}</strong> • Evaluated automatically by AI Architecture Engine
            </p>
          </div>
        </div>

        {/* Action: Open in editor */}
        {diagram?.id && (
          <button
            className="btn btn-primary"
            onClick={() => navigate(`/editor/${diagram.id}`)}
            style={{ display: 'inline-flex', alignItems: 'center', gap: '8px', height: '36px' }}
          >
            <Edit3 size={15} />
            <span>Open in Editor</span>
          </button>
        )}
      </div>

      {/* Loading state */}
      {loading && (
        <div style={{
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          padding: '100px 0',
          gap: '10px',
          color: 'var(--text-secondary)',
          fontSize: '14px',
        }}>
          <Loader2 size={24} className="animate-spin" color="var(--accent-primary)" />
          <span>Loading AI architecture review report...</span>
        </div>
      )}

      {/* Error state */}
      {!loading && error && (
        <div style={{
          padding: '16px',
          backgroundColor: 'var(--color-danger-bg)',
          border: '1px solid #fecaca',
          borderRadius: 'var(--radius-md)',
          color: 'var(--color-danger)',
          display: 'flex',
          alignItems: 'center',
          gap: '10px',
        }}>
          <AlertCircle size={18} />
          <span>{error}</span>
        </div>
      )}

      {/* Main Content Grid */}
      {!loading && !error && (
        <div style={{ display: 'grid', gridTemplateColumns: '320px 1fr', gap: '20px', alignItems: 'start' }}>
          {/* Left Column: Summary Card & Score */}
          <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
            {/* Score Card */}
            <div className="card" style={{ padding: '24px', textAlign: 'center', backgroundColor: '#ffffff' }}>
              <div style={{
                width: '48px',
                height: '48px',
                borderRadius: '12px',
                backgroundColor: 'var(--accent-blue-light)',
                color: 'var(--accent-primary)',
                display: 'inline-flex',
                alignItems: 'center',
                justifyContent: 'center',
                marginBottom: '12px',
              }}>
                <Award size={26} />
              </div>

              <h3 style={{ fontSize: '14px', fontWeight: 600, color: 'var(--text-secondary)', marginBottom: '8px' }}>
                Architecture Quality Score
              </h3>

              <div style={{
                fontSize: '42px',
                fontWeight: 800,
                color: getScoreColor(scoreOutOf10),
                lineHeight: 1,
                marginBottom: '8px',
              }}>
                {scoreOutOf10}<span style={{ fontSize: '20px', color: 'var(--text-muted)' }}>/10</span>
              </div>

              <p style={{ fontSize: '12px', color: 'var(--text-muted)' }}>
                {scoreOutOf10 >= 8.0
                  ? 'Architecture follows design standards and consistency.'
                  : 'Identified areas for refactoring and pattern improvements.'}
              </p>
            </div>

            {/* Quick Tabs Menu */}
            <div className="card" style={{ padding: '8px', backgroundColor: '#ffffff' }}>
              <button
                onClick={() => setActiveTab('report')}
                style={{
                  width: '100%',
                  display: 'flex',
                  alignItems: 'center',
                  gap: '10px',
                  padding: '10px 14px',
                  borderRadius: 'var(--radius-md)',
                  border: 'none',
                  backgroundColor: activeTab === 'report' ? 'var(--accent-blue-light)' : 'transparent',
                  color: activeTab === 'report' ? 'var(--accent-primary)' : 'var(--text-primary)',
                  fontWeight: activeTab === 'report' ? 600 : 500,
                  fontSize: '13px',
                  cursor: 'pointer',
                  textAlign: 'left',
                  transition: 'all 0.15s ease',
                }}
              >
                <FileCheck2 size={16} />
                <span>Detailed Review Report</span>
              </button>

              <button
                onClick={() => setActiveTab('refactor')}
                style={{
                  width: '100%',
                  display: 'flex',
                  alignItems: 'center',
                  gap: '10px',
                  padding: '10px 14px',
                  borderRadius: 'var(--radius-md)',
                  border: 'none',
                  backgroundColor: activeTab === 'refactor' ? 'var(--accent-blue-light)' : 'transparent',
                  color: activeTab === 'refactor' ? 'var(--accent-primary)' : 'var(--text-primary)',
                  fontWeight: activeTab === 'refactor' ? 600 : 500,
                  fontSize: '13px',
                  cursor: 'pointer',
                  textAlign: 'left',
                  marginTop: '4px',
                  transition: 'all 0.15s ease',
                }}
              >
                <Code2 size={16} />
                <span>AI-Refactored Diagram (Mermaid)</span>
              </button>

              <button
                onClick={() => setActiveTab('chat')}
                style={{
                  width: '100%',
                  display: 'flex',
                  alignItems: 'center',
                  gap: '10px',
                  padding: '10px 14px',
                  borderRadius: 'var(--radius-md)',
                  border: 'none',
                  backgroundColor: activeTab === 'chat' ? 'var(--accent-blue-light)' : 'transparent',
                  color: activeTab === 'chat' ? 'var(--accent-primary)' : 'var(--text-primary)',
                  fontWeight: activeTab === 'chat' ? 600 : 500,
                  fontSize: '13px',
                  cursor: 'pointer',
                  textAlign: 'left',
                  marginTop: '4px',
                  transition: 'all 0.15s ease',
                }}
              >
                <MessageSquare size={16} />
                <span>Chat with AI Assistant</span>
              </button>
            </div>
          </div>

          {/* Right Column: Tab View Content */}
          <div style={{ minHeight: '600px' }}>
            {/* TAB 1: Markdown Report */}
            {activeTab === 'report' && (
              <div className="card" style={{ padding: '28px', backgroundColor: '#ffffff' }}>
                <MarkdownReportViewer content={reviewDetails} />
              </div>
            )}

            {/* TAB 2: Refactored Mermaid Diagram */}
            {activeTab === 'refactor' && (
              <div className="card" style={{ padding: '24px', backgroundColor: '#ffffff' }}>
                <div style={{
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'space-between',
                  marginBottom: '20px',
                  paddingBottom: '14px',
                  borderBottom: '1px solid var(--border-color)',
                }}>
                  <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                    <Code2 size={18} color="var(--accent-primary)" />
                    <h2 style={{ fontSize: '16.5px', fontWeight: 600, color: 'var(--text-primary)' }}>
                      AI-Refactored Architecture Diagram
                    </h2>
                  </div>

                  {refactoredMermaid && (
                    <button
                      className="btn btn-outline btn-sm"
                      onClick={handleCopyCode}
                      style={{ gap: '6px', fontSize: '12px' }}
                    >
                      {copied ? <Check size={14} color="var(--color-success)" /> : <Copy size={14} />}
                      <span>{copied ? 'Copied!' : 'Copy Mermaid Code'}</span>
                    </button>
                  )}
                </div>

                {refactoredMermaid ? (
                  <div>
                    {/* Visual Diagram Render */}
                    <div style={{
                      borderRadius: 'var(--radius-md)',
                      border: '1px solid var(--border-color)',
                      marginBottom: '20px',
                      overflow: 'hidden',
                    }}>
                      <MermaidRenderer code={refactoredMermaid} id="ai-refactored-mermaid" minHeight="420px" />
                    </div>

                    {/* Raw Code Block */}
                    <div>
                      <h4 style={{ fontSize: '13px', fontWeight: 600, color: 'var(--text-secondary)', marginBottom: '8px' }}>
                        Mermaid Source Code:
                      </h4>
                      <pre style={{
                        padding: '16px',
                        backgroundColor: '#0f172a',
                        color: '#f8fafc',
                        borderRadius: 'var(--radius-md)',
                        fontFamily: 'var(--font-mono)',
                        fontSize: '12.5px',
                        lineHeight: 1.5,
                        overflowX: 'auto',
                      }}>
                        {refactoredMermaid}
                      </pre>
                    </div>
                  </div>
                ) : (
                  <div style={{ padding: '60px', textAlign: 'center', color: 'var(--text-muted)' }}>
                    No refactored Mermaid source code found for this version.
                  </div>
                )}
              </div>
            )}

            {/* TAB 3: Interactive Chat Panel */}
            {activeTab === 'chat' && (
              <div style={{ height: '620px' }}>
                <AIChatPanel sessionId={report?.id || diagram?.id} />
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
