import React, { useEffect, useRef, useState } from 'react';
import mermaid from 'mermaid';
import {
  AlertCircle,
  RefreshCw,
  ZoomIn,
  ZoomOut,
  RotateCcw,
  Maximize2,
  Minimize2,
  Download,
  Sliders,
} from 'lucide-react';

mermaid.initialize({
  startOnLoad: false,
  theme: 'default',
  securityLevel: 'loose',
  fontFamily: 'Inter, sans-serif',
  suppressErrorRendering: true,
  flowchart: { htmlLabels: true, curve: 'basis', useMaxWidth: true },
  sequence: { showSequenceNumbers: true, useMaxWidth: true },
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

export default function MermaidRenderer({
  code,
  id = 'mermaid-chart',
  showControls = true,
  minHeight = '360px',
}) {
  const containerRef = useRef(null);
  const [svgContent, setSvgContent] = useState('');
  const [renderError, setRenderError] = useState('');
  const [rendering, setRendering] = useState(false);

  // Zoom & Pan state
  const [zoom, setZoom] = useState(1);
  const [fitToWidth, setFitToWidth] = useState(true);
  const [isFullscreen, setIsFullscreen] = useState(false);

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
        await mermaid.parse(cleaned);
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

  const handleZoomIn = () => {
    setFitToWidth(false);
    setZoom((z) => Math.min(+(z + 0.15).toFixed(2), 2.5));
  };

  const handleZoomOut = () => {
    setFitToWidth(false);
    setZoom((z) => Math.max(+(z - 0.15).toFixed(2), 0.35));
  };

  const handleResetZoom = () => {
    setZoom(1);
    setFitToWidth(false);
  };

  const handleToggleFit = () => {
    setFitToWidth((prev) => !prev);
    setZoom(1);
  };

  const handleDownloadSvg = () => {
    if (!svgContent) return;
    const blob = new Blob([svgContent], { type: 'image/svg+xml;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `diagram-${id}.svg`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  };

  if (rendering && !svgContent) {
    return (
      <div style={{
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        padding: '50px 20px',
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
        margin: '20px auto',
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
      <div style={{ padding: '40px', color: 'var(--text-muted)', fontSize: '13px', textAlign: 'center' }}>
        No diagram content available to render.
      </div>
    );
  }

  const containerContent = (
    <div style={{
      position: isFullscreen ? 'fixed' : 'relative',
      top: isFullscreen ? 0 : 'auto',
      left: isFullscreen ? 0 : 'auto',
      width: isFullscreen ? '100vw' : '100%',
      height: isFullscreen ? '100vh' : 'auto',
      zIndex: isFullscreen ? 9999 : 1,
      backgroundColor: isFullscreen ? '#ffffff' : 'transparent',
      display: 'flex',
      flexDirection: 'column',
    }}>
      {/* Interactive Controls Toolbar */}
      {showControls && (
        <div style={{
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          padding: '8px 12px',
          backgroundColor: '#f8fafc',
          borderBottom: '1px solid var(--border-color)',
          borderRadius: isFullscreen ? '0' : '8px 8px 0 0',
          gap: '10px',
          flexWrap: 'wrap',
        }}>
          {/* Status badge */}
          <div style={{ display: 'flex', alignItems: 'center', gap: '8px', fontSize: '12px', color: 'var(--text-muted)' }}>
            <span style={{
              display: 'inline-flex',
              alignItems: 'center',
              padding: '2px 8px',
              borderRadius: '12px',
              backgroundColor: fitToWidth ? 'var(--accent-blue-light)' : '#f1f5f9',
              color: fitToWidth ? 'var(--accent-primary)' : 'var(--text-secondary)',
              fontWeight: 600,
              fontSize: '11px',
            }}>
              {fitToWidth ? 'Fit to Screen' : `${Math.round(zoom * 100)}%`}
            </span>
          </div>

          {/* Action Buttons */}
          <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
            <button
              onClick={handleToggleFit}
              title={fitToWidth ? 'Switch to Original Scale' : 'Fit Entire Diagram to Screen'}
              className="btn btn-outline btn-sm"
              style={{
                fontSize: '11.5px',
                padding: '4px 8px',
                height: '28px',
                backgroundColor: fitToWidth ? 'var(--accent-blue-light)' : 'transparent',
                borderColor: fitToWidth ? 'var(--accent-primary)' : 'var(--border-color)',
                color: fitToWidth ? 'var(--accent-primary)' : 'var(--text-primary)',
              }}
            >
              <span>Fit Width</span>
            </button>

            <button
              onClick={handleZoomOut}
              title="Zoom Out (-)"
              className="btn btn-outline btn-sm"
              style={{ padding: '4px 6px', height: '28px' }}
            >
              <ZoomOut size={14} />
            </button>

            <button
              onClick={handleResetZoom}
              title="Reset 100%"
              className="btn btn-outline btn-sm"
              style={{ padding: '4px 6px', height: '28px' }}
            >
              <RotateCcw size={14} />
            </button>

            <button
              onClick={handleZoomIn}
              title="Zoom In (+)"
              className="btn btn-outline btn-sm"
              style={{ padding: '4px 6px', height: '28px' }}
            >
              <ZoomIn size={14} />
            </button>

            <button
              onClick={handleDownloadSvg}
              title="Download Diagram as SVG"
              className="btn btn-outline btn-sm"
              style={{ padding: '4px 6px', height: '28px' }}
            >
              <Download size={14} />
            </button>

            <button
              onClick={() => setIsFullscreen((prev) => !prev)}
              title={isFullscreen ? 'Exit Fullscreen' : 'Fullscreen'}
              className="btn btn-outline btn-sm"
              style={{ padding: '4px 6px', height: '28px' }}
            >
              {isFullscreen ? <Minimize2 size={14} /> : <Maximize2 size={14} />}
            </button>
          </div>
        </div>
      )}

      {/* SVG Canvas Area */}
      <div
        style={{
          width: '100%',
          minHeight: isFullscreen ? 'calc(100vh - 50px)' : minHeight,
          maxHeight: isFullscreen ? 'calc(100vh - 50px)' : '75vh',
          overflow: 'auto',
          display: 'flex',
          justifyContent: fitToWidth ? 'center' : 'flex-start',
          alignItems: 'center',
          padding: '24px',
          backgroundColor: '#ffffff',
          borderRadius: isFullscreen ? '0' : '0 0 8px 8px',
        }}
      >
        <div
          ref={containerRef}
          style={{
            transform: fitToWidth ? 'none' : `scale(${zoom})`,
            transformOrigin: 'top center',
            transition: 'transform 0.15s ease-out',
            width: fitToWidth ? '100%' : 'max-content',
            display: 'flex',
            justifyContent: 'center',
          }}
          className={fitToWidth ? 'mermaid-fit-container' : 'mermaid-raw-container'}
          dangerouslySetInnerHTML={{ __html: svgContent }}
        />
      </div>

      <style>{`
        .mermaid-fit-container svg {
          width: 100% !important;
          max-width: 100% !important;
          height: auto !important;
          display: block;
          margin: 0 auto;
        }
        .mermaid-raw-container svg {
          max-width: none !important;
          height: auto !important;
          display: block;
        }
      `}</style>
    </div>
  );

  return containerContent;
}
