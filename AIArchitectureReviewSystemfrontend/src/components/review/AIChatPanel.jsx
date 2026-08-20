import React, { useState, useEffect, useRef } from 'react';
import { Send, Bot, User, Loader2, Sparkles } from 'lucide-react';
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
        if (res && Array.isArray(res.data) && res.data.length > 0) {
          setMessages(res.data.map((m) => ({
            role: m.isUser ? 'user' : 'assistant',
            content: m.content || m.message,
          })));
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
      const reply = res?.data?.reply || res?.data?.message || res?.data || 'Request received.';
      
      setMessages((prev) => [...prev, { role: 'assistant', content: typeof reply === 'string' ? reply : JSON.stringify(reply) }]);
    } catch (err) {
      console.error('Chat error:', err);
      setMessages((prev) => [
        ...prev,
        {
          role: 'assistant',
          content: 'Sorry, I am currently unable to reach the AI Service. ' + (err.response?.data || err.message),
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
        padding: '14px 18px',
        borderBottom: '1px solid var(--border-color)',
        display: 'flex',
        alignItems: 'center',
        gap: '10px',
        backgroundColor: '#fafafa',
      }}>
        <div style={{
          width: '32px',
          height: '32px',
          borderRadius: '8px',
          backgroundColor: 'var(--accent-blue-light)',
          color: 'var(--accent-primary)',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
        }}>
          <Sparkles size={16} />
        </div>
        <div>
          <h4 style={{ fontSize: '13.5px', fontWeight: 600, color: 'var(--text-primary)' }}>
            AI Architecture Assistant
          </h4>
          <span style={{ fontSize: '11px', color: 'var(--color-success)', fontWeight: 500 }}>
            ● Ready to assist
          </span>
        </div>
      </div>

      {/* Message list */}
      <div style={{
        flex: 1,
        padding: '16px',
        overflowY: 'auto',
        display: 'flex',
        flexDirection: 'column',
        gap: '12px',
      }}>
        {messages.map((msg, idx) => {
          const isUser = msg.role === 'user';
          return (
            <div
              key={idx}
              style={{
                display: 'flex',
                gap: '8px',
                alignItems: 'flex-start',
                flexDirection: isUser ? 'row-reverse' : 'row',
              }}
            >
              <div style={{
                width: '28px',
                height: '28px',
                borderRadius: '50%',
                backgroundColor: isUser ? 'var(--accent-primary)' : 'var(--accent-blue-light)',
                color: isUser ? '#ffffff' : 'var(--accent-primary)',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                flexShrink: 0,
              }}>
                {isUser ? <User size={14} /> : <Bot size={15} />}
              </div>

              <div style={{
                maxWidth: '82%',
                padding: '10px 14px',
                borderRadius: '12px',
                fontSize: '13px',
                lineHeight: 1.45,
                backgroundColor: isUser ? 'var(--accent-primary)' : 'var(--bg-main)',
                color: isUser ? '#ffffff' : 'var(--text-primary)',
                border: isUser ? 'none' : '1px solid var(--border-color)',
                whiteSpace: 'pre-wrap',
                wordBreak: 'break-word',
              }}>
                {msg.content}
              </div>
            </div>
          );
        })}

        {sending && (
          <div style={{ display: 'flex', gap: '8px', alignItems: 'center' }}>
            <div style={{
              width: '28px',
              height: '28px',
              borderRadius: '50%',
              backgroundColor: 'var(--accent-blue-light)',
              color: 'var(--accent-primary)',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
            }}>
              <Bot size={15} />
            </div>
            <div style={{
              padding: '8px 14px',
              borderRadius: '12px',
              backgroundColor: 'var(--bg-main)',
              border: '1px solid var(--border-color)',
              display: 'flex',
              alignItems: 'center',
              gap: '6px',
              fontSize: '12.5px',
              color: 'var(--text-secondary)',
            }}>
              <Loader2 size={14} className="animate-spin" />
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
          padding: '12px',
          borderTop: '1px solid var(--border-color)',
          display: 'flex',
          gap: '8px',
          backgroundColor: '#fafafa',
        }}
      >
        <input
          type="text"
          className="input-text"
          placeholder="Ask AI about architectural choices, patterns, refactoring..."
          value={input}
          onChange={(e) => setInput(e.target.value)}
          disabled={sending}
          style={{ flex: 1, height: '38px', fontSize: '13px' }}
        />
        <button
          type="submit"
          className="btn btn-primary"
          disabled={!input.trim() || sending}
          style={{ height: '38px', width: '38px', padding: 0, display: 'flex', alignItems: 'center', justifyContent: 'center' }}
        >
          <Send size={15} />
        </button>
      </form>
    </div>
  );
}
