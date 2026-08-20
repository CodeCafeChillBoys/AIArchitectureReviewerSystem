import React, { useEffect, useRef, useState } from 'react';
import mermaid from 'mermaid';
import { AlertCircle, RefreshCw } from 'lucide-react';

mermaid.initialize({
  startOnLoad: false,
  theme: 'default',
  securityLevel: 'loose',
  fontFamily: 'Inter, sans-serif',
  suppressErrorRendering: true, // Ngăn Mermaid tự chèn SVG lỗi vào DOM
  flowchart: { htmlLabels: true, curve: 'basis' },
  sequence: { showSequenceNumbers: true },
});

/**
 * Trích xuất và làm sạch mã Mermaid từ chuỗi đầu vào (hỗ trợ cả khi bọc trong markdown ```mermaid)
 */
export function cleanMermaidCode(input) {
  if (!input || typeof input !== 'string') return '';
  let text = input.trim();

  // 1. Tìm khối ```mermaid ... ```
  const mermaidBlockRegex = /```(?:mermaid)?\s*([\s\S]*?)```/i;
  const match = text.match(mermaidBlockRegex);
  if (match && match[1] && match[1].trim()) {
    text = match[1].trim();
  }

  // 2. Loại bỏ backticks thừa
  text = text.replace(/^```(?:mermaid)?\s*/i, '').replace(/```$/, '').trim();

  // 3. Tìm dòng bắt đầu hợp lệ của Mermaid nếu còn text thừa ở trước
  const validStarters = [
    'graph',
    'flowchart',
    'sequencediagram',
    'classdiagram',
    'statediagram',
    'erdiagram',
    'journey',
    'gantt',
    'pie',
    'quadrantchart',
    'mindmap',
    'timeline',
    'zenuml',
    'c4context',
    'block-beta',
    'architecture-beta',
    '%%',
  ];

  const lines = text.split('\n');
  const starterIndex = lines.findIndex((l) =>
    validStarters.some((s) => l.trim().toLowerCase().startsWith(s))
  );

  if (starterIndex !== -1) {
    return lines.slice(starterIndex).join('\n').trim();
  }

  return text;
}

export default function MermaidRenderer({ code, id = 'mermaid-chart' }) {
  const containerRef = useRef(null);
  const [svgContent, setSvgContent] = useState('');
  const [renderError, setRenderError] = useState('');
  const [rendering, setRendering] = useState(false);

  useEffect(() => {
    let isMounted = true;

    const renderChart = async () => {
      const cleaned = cleanMermaidCode(code);

      if (!cleaned) {
        if (isMounted) {
          setSvgContent('');
          setRenderError('');
          setRendering(false);
        }
        return;
      }

      if (isMounted) {
        setRendering(true);
        setRenderError('');
      }

      const uniqueId = `mmd-${id}-${Math.random().toString(36).substring(2, 9)}`;

      try {
        // Kiểm tra parse cú pháp an toàn trước
        await mermaid.parse(cleaned);

        // Render ra SVG
        const { svg } = await mermaid.render(uniqueId, cleaned);
        if (isMounted) {
          setSvgContent(svg);
          setRenderError('');
        }
      } catch (err) {
        console.warn('Mermaid parse/render error:', err);
        if (isMounted) {
          setRenderError(err.message || 'Cú pháp Mermaid không đúng định dạng.');
          setSvgContent('');
        }
      } finally {
        // Dọn dẹp DOM node rác do Mermaid có thể tạo trong document.body
        const garbageNode = document.getElementById(uniqueId) || document.getElementById(`d${uniqueId}`);
        if (garbageNode && garbageNode.parentNode) {
          garbageNode.parentNode.removeChild(garbageNode);
        }

        if (isMounted) {
          setRendering(false);
        }
      }
    };

    renderChart();

    return () => {
      isMounted = false;
    };
  }, [code, id]);

  if (rendering && !svgContent) {
    return (
      <div style={{
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        padding: '30px',
        color: 'var(--text-muted)',
        gap: '8px',
        fontSize: '13px',
      }}>
        <RefreshCw size={16} className="animate-spin" />
        <span>Rendering diagram...</span>
      </div>
    );
  }

  if (renderError) {
    return (
      <div style={{
        padding: '14px 18px',
        backgroundColor: '#fff7ed',
        border: '1px solid #fed7aa',
        borderRadius: 'var(--radius-md)',
        color: '#c2410c',
        fontSize: '13px',
        width: '100%',
        maxWidth: '600px',
        margin: '0 auto',
      }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '8px', fontWeight: 600, marginBottom: '6px' }}>
          <AlertCircle size={16} />
          <span>Mermaid Syntax Notice:</span>
        </div>
        <p style={{ fontSize: '12.5px', color: '#9a3412', marginBottom: '8px' }}>
          Please verify the diagram initialization header (e.g. <code>sequenceDiagram</code>, <code>classDiagram</code>, or <code>graph TD</code>).
        </p>
      </div>
    );
  }

  if (!svgContent) {
    return (
      <div style={{ padding: '30px', color: 'var(--text-muted)', fontSize: '13px', textAlign: 'center' }}>
        No diagram content available to render.
      </div>
    );
  }

  return (
    <div
      ref={containerRef}
      style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        width: '100%',
        height: '100%',
        minHeight: '220px',
        overflow: 'auto',
      }}
      dangerouslySetInnerHTML={{ __html: svgContent }}
    />
  );
}
