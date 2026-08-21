import React, { useState } from 'react';
import ReactMarkdown from 'react-markdown';
import remarkGfm from 'remark-gfm';
import {
  AlertTriangle,
  AlertOctagon,
  Info,
  CheckCircle2,
  Copy,
  Check,
  Code2,
} from 'lucide-react';

/**
 * Render Severity Badge (Critical, High, Medium, Minor, Low)
 */
function SeverityBadge({ level }) {
  const text = (typeof level === 'string' ? level : '').trim().toLowerCase();

  if (text.includes('critical') || text.includes('nghiêm trọng')) {
    return (
      <span style={{
        display: 'inline-flex',
        alignItems: 'center',
        gap: '5px',
        fontSize: '12px',
        fontWeight: 600,
        padding: '3px 10px',
        borderRadius: '20px',
        backgroundColor: '#fef2f2',
        color: '#dc2626',
        border: '1px solid #fecaca',
      }}>
        <AlertOctagon size={13} />
        <span>Critical</span>
      </span>
    );
  }

  if (text.includes('high') || text.includes('cao')) {
    return (
      <span style={{
        display: 'inline-flex',
        alignItems: 'center',
        gap: '5px',
        fontSize: '12px',
        fontWeight: 600,
        padding: '3px 10px',
        borderRadius: '20px',
        backgroundColor: '#fff1f2',
        color: '#e11d48',
        border: '1px solid #fecdd3',
      }}>
        <AlertOctagon size={13} />
        <span>High</span>
      </span>
    );
  }

  if (text.includes('medium') || text.includes('trung bình') || text.includes('warning')) {
    return (
      <span style={{
        display: 'inline-flex',
        alignItems: 'center',
        gap: '5px',
        fontSize: '12px',
        fontWeight: 600,
        padding: '3px 10px',
        borderRadius: '20px',
        backgroundColor: '#fffbeb',
        color: '#d97706',
        border: '1px solid #fde68a',
      }}>
        <AlertTriangle size={13} />
        <span>Medium</span>
      </span>
    );
  }

  if (text.includes('minor') || text.includes('low') || text.includes('nhỏ') || text.includes('thấp')) {
    return (
      <span style={{
        display: 'inline-flex',
        alignItems: 'center',
        gap: '5px',
        fontSize: '12px',
        fontWeight: 600,
        padding: '3px 10px',
        borderRadius: '20px',
        backgroundColor: '#eff6ff',
        color: '#2563eb',
        border: '1px solid #bfdbfe',
      }}>
        <Info size={13} />
        <span>Minor</span>
      </span>
    );
  }

  if (text.includes('passed') || text.includes('good') || text.includes('tốt')) {
    return (
      <span style={{
        display: 'inline-flex',
        alignItems: 'center',
        gap: '5px',
        fontSize: '12px',
        fontWeight: 600,
        padding: '3px 10px',
        borderRadius: '20px',
        backgroundColor: '#f0fdf4',
        color: '#16a34a',
        border: '1px solid #bbf7d0',
      }}>
        <CheckCircle2 size={13} />
        <span>Passed</span>
      </span>
    );
  }

  return <span>{level}</span>;
}

/**
 * Code block with copy button
 */
function CodeBlock({ language, value }) {
  const [copied, setCopied] = useState(false);

  const handleCopy = () => {
    navigator.clipboard.writeText(value);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  return (
    <div style={{
      position: 'relative',
      margin: '14px 0',
      borderRadius: '8px',
      overflow: 'hidden',
      border: '1px solid #334155',
      backgroundColor: '#0f172a',
    }}>
      {/* Header bar */}
      <div style={{
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        padding: '6px 14px',
        backgroundColor: '#1e293b',
        borderBottom: '1px solid #334155',
        fontSize: '12px',
        color: '#94a3b8',
      }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
          <Code2 size={14} color="#38bdf8" />
          <span style={{ textTransform: 'uppercase', fontWeight: 600 }}>{language || 'code'}</span>
        </div>
        <button
          onClick={handleCopy}
          style={{
            display: 'inline-flex',
            alignItems: 'center',
            gap: '4px',
            background: 'transparent',
            border: 'none',
            color: copied ? '#4ade80' : '#94a3b8',
            cursor: 'pointer',
            fontSize: '11.5px',
            padding: '2px 6px',
            borderRadius: '4px',
            transition: 'color 0.15s',
          }}
        >
          {copied ? <Check size={13} /> : <Copy size={13} />}
          <span>{copied ? 'Copied!' : 'Copy'}</span>
        </button>
      </div>

      {/* Code body */}
      <pre style={{
        margin: 0,
        padding: '14px 16px',
        overflowX: 'auto',
        fontFamily: 'Consolas, Monaco, "Courier New", monospace',
        fontSize: '13px',
        lineHeight: 1.6,
        color: '#f8fafc',
      }}>
        <code>{value}</code>
      </pre>
    </div>
  );
}

/**
 * Preprocess raw text or tab-separated tables into clean Markdown
 */
function normalizeMarkdown(rawContent) {
  if (!rawContent || typeof rawContent !== 'string') return '';

  let content = rawContent.trim();
  const lines = content.split('\n');
  const normalizedLines = [];
  let inTabTable = false;

  for (let i = 0; i < lines.length; i++) {
    const line = lines[i];

    // Detect tab-separated lines (e.g. copied tables)
    if (line.includes('\t')) {
      const cols = line.split('\t').map((c) => c.trim()).filter(Boolean);
      if (cols.length >= 2) {
        normalizedLines.push(`| ${cols.join(' | ')} |`);
        if (!inTabTable) {
          inTabTable = true;
          normalizedLines.push(`| ${cols.map(() => '---').join(' | ')} |`);
        }
        continue;
      }
    } else {
      inTabTable = false;
    }

    normalizedLines.push(line);
  }

  return normalizedLines.join('\n');
}

export default function MarkdownReportViewer({ content }) {
  if (!content || !content.trim()) {
    return (
      <div style={{ padding: '40px', textAlign: 'center', color: 'var(--text-muted)' }}>
        No review report data to display.
      </div>
    );
  }

  const normalized = normalizeMarkdown(content);

  return (
    <div className="markdown-report-container" style={{
      color: 'var(--text-primary)',
      fontSize: '14px',
      lineHeight: 1.65,
      fontFamily: 'var(--font-sans)',
    }}>
      <ReactMarkdown
        remarkPlugins={[remarkGfm]}
        components={{
          // Headers
          h1: ({ children }) => (
            <h1 style={{
              fontSize: '20px',
              fontWeight: 700,
              color: 'var(--text-primary)',
              marginTop: '10px',
              marginBottom: '16px',
              paddingBottom: '8px',
              borderBottom: '2px solid var(--border-color)',
            }}>
              {children}
            </h1>
          ),
          h2: ({ children }) => (
            <h2 style={{
              fontSize: '16px',
              fontWeight: 700,
              color: 'var(--accent-primary)',
              marginTop: '28px',
              marginBottom: '14px',
              padding: '8px 12px',
              backgroundColor: 'var(--accent-blue-light)',
              borderRadius: '6px',
              borderLeft: '4px solid var(--accent-primary)',
              display: 'flex',
              alignItems: 'center',
              gap: '8px',
            }}>
              {children}
            </h2>
          ),
          h3: ({ children }) => (
            <h3 style={{
              fontSize: '14.5px',
              fontWeight: 600,
              color: 'var(--text-primary)',
              marginTop: '18px',
              marginBottom: '8px',
            }}>
              {children}
            </h3>
          ),
          // Tables
          table: ({ children }) => (
            <div style={{
              overflowX: 'auto',
              borderRadius: '8px',
              border: '1px solid var(--border-color)',
              margin: '16px 0 24px 0',
              backgroundColor: '#ffffff',
              boxShadow: '0 1px 3px rgba(0,0,0,0.03)',
            }}>
              <table style={{
                width: '100%',
                borderCollapse: 'collapse',
                textAlign: 'left',
                fontSize: '13px',
              }}>
                {children}
              </table>
            </div>
          ),
          thead: ({ children }) => (
            <thead style={{
              backgroundColor: '#f8fafc',
              borderBottom: '2px solid var(--border-color)',
            }}>
              {children}
            </thead>
          ),
          th: ({ children }) => (
            <th style={{
              padding: '12px 16px',
              fontWeight: 600,
              color: 'var(--text-secondary)',
              fontSize: '12px',
              textTransform: 'uppercase',
              letterSpacing: '0.04em',
            }}>
              {children}
            </th>
          ),
          tr: ({ children }) => (
            <tr style={{
              borderBottom: '1px solid var(--border-color)',
              transition: 'background-color 0.15s ease',
            }}>
              {children}
            </tr>
          ),
          td: ({ children }) => {
            const rawText = Array.isArray(children)
              ? children.map((c) => (typeof c === 'string' ? c : '')).join('')
              : typeof children === 'string'
              ? children
              : '';

            const isSeverity =
              rawText.toLowerCase().includes('critical') ||
              rawText.toLowerCase().includes('high') ||
              rawText.toLowerCase().includes('medium') ||
              rawText.toLowerCase().includes('minor') ||
              rawText.toLowerCase().includes('low') ||
              rawText.toLowerCase().includes('nghiêm trọng') ||
              rawText.toLowerCase().includes('trung bình');

            return (
              <td style={{
                padding: '12px 16px',
                verticalAlign: 'top',
                color: 'var(--text-primary)',
                lineHeight: 1.5,
              }}>
                {isSeverity && rawText.length < 30 ? (
                  <SeverityBadge level={rawText} />
                ) : (
                  children
                )}
              </td>
            );
          },
          // Blockquotes
          blockquote: ({ children }) => (
            <blockquote style={{
              margin: '12px 0',
              padding: '10px 16px',
              backgroundColor: '#f8fafc',
              borderLeft: '4px solid var(--accent-primary)',
              borderRadius: '0 8px 8px 0',
              color: 'var(--text-secondary)',
              fontStyle: 'normal',
              fontSize: '13.5px',
            }}>
              {children}
            </blockquote>
          ),
          // Code blocks & Inline code
          code: ({ node, inline, className, children, ...props }) => {
            const match = /language-(\w+)/.exec(className || '');
            const codeContent = String(children).replace(/\n$/, '');

            if (!inline && match) {
              return <CodeBlock language={match[1]} value={codeContent} />;
            }

            if (!inline && codeContent.includes('\n')) {
              return <CodeBlock language="" value={codeContent} />;
            }

            return (
              <code
                style={{
                  fontFamily: 'Consolas, Monaco, "Courier New", monospace',
                  fontSize: '12.5px',
                  backgroundColor: '#f1f5f9',
                  color: '#2563eb',
                  padding: '2px 6px',
                  borderRadius: '4px',
                  border: '1px solid #e2e8f0',
                  fontWeight: 500,
                }}
                {...props}
              >
                {children}
              </code>
            );
          },
          // Lists
          ul: ({ children }) => (
            <ul style={{
              paddingLeft: '22px',
              margin: '10px 0',
              display: 'flex',
              flexDirection: 'column',
              gap: '6px',
            }}>
              {children}
            </ul>
          ),
          ol: ({ children }) => (
            <ol style={{
              paddingLeft: '22px',
              margin: '10px 0',
              display: 'flex',
              flexDirection: 'column',
              gap: '6px',
            }}>
              {children}
            </ol>
          ),
          li: ({ children }) => (
            <li style={{
              color: 'var(--text-primary)',
              fontSize: '13.5px',
              lineHeight: 1.6,
            }}>
              {children}
            </li>
          ),
          // Paragraphs
          p: ({ children }) => (
            <p style={{
              margin: '8px 0',
              fontSize: '13.5px',
              lineHeight: 1.65,
              color: 'var(--text-primary)',
            }}>
              {children}
            </p>
          ),
          // Horizontal Rule
          hr: () => (
            <hr style={{
              border: 'none',
              borderTop: '1px solid var(--border-color)',
              margin: '20px 0',
            }} />
          ),
          // Strong
          strong: ({ children }) => (
            <strong style={{ fontWeight: 600, color: 'var(--text-primary)' }}>
              {children}
            </strong>
          ),
        }}
      >
        {normalized}
      </ReactMarkdown>
    </div>
  );
}
