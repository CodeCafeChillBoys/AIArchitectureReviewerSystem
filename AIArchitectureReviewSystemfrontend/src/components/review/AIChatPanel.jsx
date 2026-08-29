import React, { useState, useEffect, useRef } from 'react';
import { Send, Loader2 } from 'lucide-react';
import ReactMarkdown from 'react-markdown';
import remarkGfm from 'remark-gfm';
import { reviewService } from '../../services/reviewService';

export default function AIChatPanel({ sessionId }) {
  const [messages, setMessages] = useState([
    {
      role: 'assistant',
      content: 'Hello! I am your AI Architecture Assistant. How can I help clarify the review report or help refactor your diagram?',
    },
  ]);
  const [input, setInput] = useState('');
  const [sending, setSending] = useState(false);
  const messagesEndRef = useRef(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  // Load chat history if sessionId exists
  useEffect(() => {
    if (!sessionId) return;
    const fetchHistory = async () => {
      try {
        const res = await reviewService.getChatHistory(sessionId);
        const list = Array.isArray(res) ? res : (Array.isArray(res?.data) ? res.data : []);
        if (list.length > 0) {
          setMessages(list.map((m) => {
            const isUserMessage = m.role?.toLowerCase() === 'user' || m.isUser === true;
            return {
              role: isUserMessage ? 'user' : 'assistant',
              content: m.content || m.message || '',
            };
          }));
        }
      } catch (err) {
        console.error('Failed to load chat history:', err);
      }
    };
    fetchHistory();
  }, [sessionId]);

  const handleSend = async (e) => {
    e.preventDefault();
    if (!input.trim() || sending) return;

    const userMessage = input.trim();
    setInput('');
    setMessages((prev) => [...prev, { role: 'user', content: userMessage }]);
    setSending(true);

    try {
      const activeSession = sessionId || '00000000-0000-0000-0000-000000000000';
      const res = await reviewService.sendChatMessage(activeSession, userMessage);
      
      const reply = res?.message ?? res?.reply ?? res?.data?.message ?? res?.data?.reply ?? (typeof res === 'string' ? res : '');
      const finalReply = reply || 'Sorry, I could not generate a response. Please try again.';
      
      setMessages((prev) => [
        ...prev,
        { role: 'assistant', content: typeof finalReply === 'string' ? finalReply : JSON.stringify(finalReply) }
      ]);
    } catch (err) {
      console.error('Chat error:', err);
      setMessages((prev) => [
        ...prev,
        {
          role: 'assistant',
          content: 'Sorry, I am currently unable to reach the AI Service. ' + (err.response?.data?.message || err.response?.data || err.message),
        },
      ]);
    } finally {
      setSending(false);
    }
  };

  return (
    <div style={{
      display: 'flex',
      flexDirection: 'column',
      height: '100%',
      backgroundColor: '#ffffff',
      borderRadius: 'var(--radius-lg)',
      border: '1px solid var(--border-color)',
      overflow: 'hidden',
    }}>
      {/* Chat header */}
      <div style={{
        padding: '14px 20px',
        borderBottom: '1px solid var(--border-color)',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        backgroundColor: '#ffffff',
      }}>
        <div>
          <h4 style={{ fontSize: '14px', fontWeight: 600, color: 'var(--text-primary)', margin: 0 }}>
            AI Architecture Assistant
          </h4>
          <span style={{ fontSize: '11.5px', color: 'var(--color-success)', fontWeight: 500 }}>
            ● Ready to assist
          </span>
        </div>
      </div>

      {/* Message list */}
      <div style={{
        flex: 1,
        padding: '20px',
        overflowY: 'auto',
        display: 'flex',
        flexDirection: 'column',
        gap: '16px',
      }}>
        {messages.map((msg, idx) => {
          const isUser = msg.role === 'user';
          return (
            <div
              key={idx}
              style={{
                display: 'flex',
                justifyContent: isUser ? 'flex-end' : 'flex-start',
                width: '100%',
              }}
            >
              <div style={{
                maxWidth: '80%',
                padding: '12px 16px',
                borderRadius: isUser ? '16px 16px 4px 16px' : '16px 16px 16px 4px',
                fontSize: '13.5px',
                lineHeight: 1.55,
                backgroundColor: isUser ? 'var(--accent-primary)' : '#f1f5f9',
                color: isUser ? '#ffffff' : 'var(--text-primary)',
                border: isUser ? 'none' : '1px solid #e2e8f0',
                wordBreak: 'break-word',
                boxShadow: isUser ? '0 2px 4px rgba(37, 99, 235, 0.15)' : 'none',
              }}>
                {isUser ? (
                  <div style={{ whiteSpace: 'pre-wrap' }}>{msg.content}</div>
                ) : (
                  <div className="chat-ai-markdown" style={{ fontSize: '13.5px', lineHeight: 1.6 }}>
                    <ReactMarkdown
                      remarkPlugins={[remarkGfm]}
                      components={{
                        p: ({ children }) => <p style={{ margin: '4px 0' }}>{children}</p>,
                        ul: ({ children }) => <ul style={{ paddingLeft: '18px', margin: '6px 0' }}>{children}</ul>,
                        ol: ({ children }) => <ol style={{ paddingLeft: '18px', margin: '6px 0' }}>{children}</ol>,
                        li: ({ children }) => <li style={{ margin: '2px 0' }}>{children}</li>,
                        code: ({ inline, children }) => (
                          <code style={{
                            fontFamily: 'Consolas, Monaco, monospace',
                            fontSize: '12px',
                            backgroundColor: '#e2e8f0',
                            color: '#0f172a',
                            padding: '2px 5px',
                            borderRadius: '4px',
                          }}>
                            {children}
                          </code>
                        ),
                        pre: ({ children }) => (
                          <pre style={{
                            padding: '10px 12px',
                            backgroundColor: '#0f172a',
                            color: '#f8fafc',
                            borderRadius: '6px',
                            overflowX: 'auto',
                            fontSize: '12.5px',
                            margin: '8px 0',
                          }}>
                            {children}
                          </pre>
                        ),
                        strong: ({ children }) => <strong style={{ fontWeight: 600 }}>{children}</strong>,
                      }}
                    >
                      {msg.content}
                    </ReactMarkdown>
                  </div>
                )}
              </div>
            </div>
          );
        })}

        {sending && (
          <div style={{ display: 'flex', justifyContent: 'flex-start' }}>
            <div style={{
              padding: '10px 16px',
              borderRadius: '16px 16px 16px 4px',
              backgroundColor: '#f1f5f9',
              border: '1px solid #e2e8f0',
              display: 'flex',
              alignItems: 'center',
              gap: '8px',
              fontSize: '13px',
              color: 'var(--text-secondary)',
            }}>
              <Loader2 size={15} className="animate-spin" color="var(--accent-primary)" />
              <span>AI is analyzing and generating response...</span>
            </div>
          </div>
        )}

        <div ref={messagesEndRef} />
      </div>

      {/* Input box */}
      <form
        onSubmit={handleSend}
        style={{
          padding: '14px 18px',
          borderTop: '1px solid var(--border-color)',
          display: 'flex',
          gap: '10px',
          backgroundColor: '#ffffff',
        }}
      >
        <input
          type="text"
          className="input-text"
          placeholder="Ask AI about architectural choices, patterns, refactoring..."
          value={input}
          onChange={(e) => setInput(e.target.value)}
          disabled={sending}
          style={{ flex: 1, height: '40px', fontSize: '13px', borderRadius: '8px' }}
        />
        <button
          type="submit"
          className="btn btn-primary"
          disabled={!input.trim() || sending}
          style={{ height: '40px', width: '40px', padding: 0, display: 'flex', alignItems: 'center', justifyContent: 'center', borderRadius: '8px' }}
        >
          <Send size={15} />
        </button>
      </form>
    </div>
  );
}
