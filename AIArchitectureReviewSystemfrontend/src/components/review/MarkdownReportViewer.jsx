import React from 'react';
import {
  AlertTriangle,
  AlertOctagon,
  Info,
  FileSpreadsheet,
  Search,
  Sparkles,
  ShieldAlert,
} from 'lucide-react';

/**
 * Translate common Vietnamese AI phrases to English for existing database reports
 */
function translatePhrase(text) {
  if (!text || typeof text !== 'string') return text;

  let res = text;

  // Headers
  res = res.replace(/A\.\s*BẢNG TỔNG HỢP LỖI\s*\(Executive Summary\)/gi, 'A. EXECUTIVE SUMMARY (Issue Overview)');
  res = res.replace(/B\.\s*PHÂN TÍCH CHI TIẾT\s*&\s*VÍ DỤ MINH HỌA\s*\(Detailed Issues & Examples\)/gi, 'B. DETAILED ISSUES & EXAMPLES');
  res = res.replace(/C\.\s*GỢI Ý TÁI CẤU TRÚC VÀ DESIGN PATTERNS/gi, 'C. REFACTORING & DESIGN PATTERNS');

  // Table Headers
  res = res.replace(/^Mức độ$/gi, 'Severity');
  res = res.replace(/^Lỗi thiết kế$/gi, 'Design Issue');
  res = res.replace(/^Thành phần\/Mối quan hệ bị lỗi$/gi, 'Component / Relationship');
  res = res.replace(/^Hệ quả \/ Nguyên lý vi phạm$/gi, 'Impact / Principle');

  // Issue Keys
  res = res.replace(/Lỗi & Vị trí/gi, 'Issue & Location');
  res = res.replace(/Vấn đề/gi, 'Problem');
  res = res.replace(/Ví dụ minh họa \(Trước khi sửa\)/gi, 'Before (Issue)');
  res = res.replace(/Giải pháp khắc phục \(Sau khi sửa\)/gi, 'After (Solution)');
  res = res.replace(/Trước khi sửa/gi, 'Before');
  res = res.replace(/Sau khi sửa/gi, 'After');
  res = res.replace(/Mối quan hệ giữa/gi, 'Relationship between');
  res = res.replace(/Phương thức/gi, 'Method');
  res = res.replace(/trong lớp/gi, 'in class');
  res = res.replace(/Lớp/gi, 'Class');

  return res;
}

/**
 * Render Severity Badge (Critical, High, Medium, Minor, Low)
 */
function SeverityBadge({ level }) {
  const text = level?.trim().toLowerCase() || '';

  if (text.includes('critical') || text.includes('high') || text.includes('nghiêm trọng')) {
    return (
      <span style={{
        display: 'inline-flex',
        alignItems: 'center',
        gap: '4px',
        fontSize: '11.5px',
        fontWeight: 600,
        padding: '3px 9px',
        borderRadius: '12px',
        backgroundColor: '#fef2f2',
        color: '#dc2626',
        border: '1px solid #fecaca',
      }}>
        <AlertOctagon size={12} />
        <span>Critical</span>
      </span>
    );
  }

  if (text.includes('medium') || text.includes('trung bình') || text.includes('warning')) {
    return (
      <span style={{
        display: 'inline-flex',
        alignItems: 'center',
        gap: '4px',
        fontSize: '11.5px',
        fontWeight: 600,
        padding: '3px 9px',
        borderRadius: '12px',
        backgroundColor: '#fffbeb',
        color: '#d97706',
        border: '1px solid #fde68a',
      }}>
        <AlertTriangle size={12} />
        <span>Medium</span>
      </span>
    );
  }

  return (
    <span style={{
      display: 'inline-flex',
      alignItems: 'center',
      gap: '4px',
      fontSize: '11.5px',
      fontWeight: 600,
      padding: '3px 9px',
      borderRadius: '12px',
      backgroundColor: '#eff6ff',
      color: '#2563eb',
      border: '1px solid #bfdbfe',
    }}>
      <Info size={12} />
      <span>{text.includes('minor') ? 'Minor' : level}</span>
    </span>
  );
}

/**
 * Render inline text with bold (**text**), code (`code`), italic (*text*)
 */
function FormattedInlineText({ text }) {
  if (!text) return null;

  const translated = translatePhrase(text);
  const parts = translated.split(/(`[^`]+`|\*\*[^*]+\*\*)/g);

  return (
    <span>
      {parts.map((part, index) => {
        if (part.startsWith('`') && part.endsWith('`')) {
          return (
            <code
              key={index}
              style={{
                fontFamily: 'var(--font-mono)',
                fontSize: '12px',
                backgroundColor: '#f1f5f9',
                color: '#2563eb',
                padding: '2px 6px',
                borderRadius: '4px',
                border: '1px solid #e2e8f0',
                margin: '0 2px',
                fontWeight: 500,
              }}
            >
              {part.slice(1, -1)}
            </code>
          );
        }

        if (part.startsWith('**') && part.endsWith('**')) {
          return (
            <strong key={index} style={{ fontWeight: 600, color: 'var(--text-primary)' }}>
              {part.slice(2, -2)}
            </strong>
          );
        }

        return <span key={index}>{part}</span>;
      })}
    </span>
  );
}

/**
 * Render Markdown Table
 */
function MarkdownTable({ lines }) {
  if (!lines || lines.length < 2) return null;

  const parseRow = (line) => {
    return line
      .replace(/^\|/, '')
      .replace(/\|$/, '')
      .split('|')
      .map((c) => c.trim());
  };

  const headers = parseRow(lines[0]);
  const rows = lines.slice(2).map(parseRow);

  return (
    <div style={{
      overflowX: 'auto',
      borderRadius: 'var(--radius-md)',
      border: '1px solid var(--border-color)',
      marginBottom: '24px',
      boxShadow: '0 1px 3px rgba(0,0,0,0.02)',
      backgroundColor: '#ffffff',
    }}>
      <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left', fontSize: '13px' }}>
        <thead>
          <tr style={{ backgroundColor: '#f8fafc', borderBottom: '1px solid var(--border-color)' }}>
            {headers.map((h, i) => (
              <th
                key={i}
                style={{
                  padding: '14px 16px',
                  fontWeight: 600,
                  color: 'var(--text-secondary)',
                  fontSize: '12.5px',
                  textTransform: 'uppercase',
                  letterSpacing: '0.03em',
                  width: i === 0 ? '110px' : i === 1 ? '24%' : i === 2 ? '28%' : 'auto',
                  minWidth: i === 0 ? '100px' : i === 1 ? '160px' : i === 2 ? '180px' : '220px',
                }}
              >
                <FormattedInlineText text={h} />
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {rows.map((row, rIdx) => (
            <tr
              key={rIdx}
              style={{
                borderBottom: rIdx === rows.length - 1 ? 'none' : '1px solid var(--border-color)',
                backgroundColor: rIdx % 2 === 0 ? '#ffffff' : '#fafafa',
                transition: 'background-color 0.15s ease',
              }}
            >
              {row.map((cell, cIdx) => (
                <td key={cIdx} style={{ padding: '14px 16px', verticalAlign: 'top', color: 'var(--text-primary)', lineHeight: 1.5 }}>
                  {cIdx === 0 ? <SeverityBadge level={cell} /> : <FormattedInlineText text={cell} />}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default function MarkdownReportViewer({ content }) {
  if (!content || !content.trim()) {
    return (
      <div style={{ padding: '40px', textAlign: 'center', color: 'var(--text-muted)' }}>
        No review report data to display.
      </div>
    );
  }

  // Parse sections
  const lines = content.split('\n');
  const elements = [];
  let currentTable = [];
  let inTable = false;

  for (let i = 0; i < lines.length; i++) {
    const line = lines[i].trim();

    // Table detection
    if (line.startsWith('|') && line.endsWith('|')) {
      inTable = true;
      currentTable.push(line);
      continue;
    } else if (inTable) {
      inTable = false;
      elements.push({ type: 'table', data: [...currentTable] });
      currentTable = [];
    }

    if (!line) continue;

    // Header 1 / 2 / 3
    if (line.startsWith('###') || line.startsWith('##') || line.startsWith('#')) {
      const rawHeaderText = line.replace(/^#+\s*/, '');
      const headerText = translatePhrase(rawHeaderText);
      let icon = <FileSpreadsheet size={18} color="var(--accent-primary)" />;

      if (headerText.toLowerCase().includes('tổng hợp') || headerText.toLowerCase().includes('executive') || headerText.toLowerCase().includes('summary')) {
        icon = <ShieldAlert size={18} color="var(--color-warning)" />;
      } else if (headerText.toLowerCase().includes('chi tiết') || headerText.toLowerCase().includes('detailed') || headerText.toLowerCase().includes('issues')) {
        icon = <Search size={18} color="var(--accent-primary)" />;
      } else if (headerText.toLowerCase().includes('tái cấu trúc') || headerText.toLowerCase().includes('pattern') || headerText.toLowerCase().includes('refactor')) {
        icon = <Sparkles size={18} color="#9333ea" />;
      }

      elements.push({ type: 'header', text: headerText, icon });
    }
    // Main Bullet point
    else if (line.startsWith('* **') || line.startsWith('- **')) {
      elements.push({ type: 'issue_title', text: line.replace(/^[*|-]\s*/, '') });
    }
    // Sub bullet point (indent)
    else if (line.startsWith('* ') || line.startsWith('- ') || line.startsWith('  * ') || line.startsWith('    * ')) {
      elements.push({ type: 'bullet', text: line.replace(/^[\s*|-]+/, '') });
    }
    // Standard paragraph
    else {
      elements.push({ type: 'paragraph', text: line });
    }
  }

  if (inTable && currentTable.length > 0) {
    elements.push({ type: 'table', data: [...currentTable] });
  }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '14px', fontFamily: 'var(--font-sans)' }}>
      {elements.map((el, idx) => {
        if (el.type === 'header') {
          return (
            <div
              key={idx}
              style={{
                display: 'flex',
                alignItems: 'center',
                gap: '10px',
                padding: '12px 16px',
                backgroundColor: 'var(--accent-blue-light)',
                borderRadius: 'var(--radius-md)',
                marginTop: idx === 0 ? 0 : '18px',
                marginBottom: '8px',
                borderLeft: '4px solid var(--accent-primary)',
              }}
            >
              {el.icon}
              <h3 style={{ fontSize: '15.5px', fontWeight: 700, color: 'var(--text-primary)', margin: 0 }}>
                {el.text}
              </h3>
            </div>
          );
        }

        if (el.type === 'table') {
          return <MarkdownTable key={idx} lines={el.data} />;
        }

        if (el.type === 'issue_title') {
          return (
            <div
              key={idx}
              style={{
                marginTop: '10px',
                padding: '12px 16px',
                backgroundColor: '#f8fafc',
                borderRadius: 'var(--radius-sm)',
                border: '1px solid var(--border-color)',
                fontSize: '14px',
                fontWeight: 600,
                color: 'var(--text-primary)',
                display: 'flex',
                alignItems: 'center',
                gap: '8px',
              }}
            >
              <AlertTriangle size={15} color="var(--color-warning)" style={{ flexShrink: 0 }} />
              <FormattedInlineText text={el.text} />
            </div>
          );
        }

        if (el.type === 'bullet') {
          const lower = el.text.toLowerCase();
          const isBefore = lower.includes('trước khi sửa') || lower.includes('before');
          const isAfter = lower.includes('sau khi sửa') || lower.includes('giải pháp') || lower.includes('after') || lower.includes('solution');

          if (isBefore) {
            return (
              <div
                key={idx}
                style={{
                  marginLeft: '16px',
                  padding: '10px 14px',
                  backgroundColor: '#fef2f2',
                  borderLeft: '3px solid #ef4444',
                  borderRadius: '0 6px 6px 0',
                  fontSize: '13px',
                  lineHeight: 1.5,
                  color: '#991b1b',
                }}
              >
                <FormattedInlineText text={el.text} />
              </div>
            );
          }

          if (isAfter) {
            return (
              <div
                key={idx}
                style={{
                  marginLeft: '16px',
                  padding: '10px 14px',
                  backgroundColor: '#f0fdf4',
                  borderLeft: '3px solid #22c55e',
                  borderRadius: '0 6px 6px 0',
                  fontSize: '13px',
                  lineHeight: 1.5,
                  color: '#166534',
                }}
              >
                <FormattedInlineText text={el.text} />
              </div>
            );
          }

          return (
            <div
              key={idx}
              style={{
                marginLeft: '20px',
                fontSize: '13.5px',
                lineHeight: 1.6,
                color: 'var(--text-secondary)',
                position: 'relative',
                paddingLeft: '14px',
              }}
            >
              <span style={{ position: 'absolute', left: 0, top: '2px', color: 'var(--accent-primary)', fontWeight: 'bold' }}>•</span>
              <FormattedInlineText text={el.text} />
            </div>
          );
        }

        return (
          <p key={idx} style={{ fontSize: '13.5px', lineHeight: 1.6, color: 'var(--text-primary)', margin: 0 }}>
            <FormattedInlineText text={el.text} />
          </p>
        );
      })}
    </div>
  );
}
