import React, { useState, useEffect } from 'react';
import { CheckCircle2, AlertCircle } from 'lucide-react';
import { adminService } from '../services/adminService';

import AdminHeader from '../components/admin/AdminHeader';
import AdminTabs from '../components/admin/AdminTabs';
import AdminActionBar from '../components/admin/AdminActionBar';
import SystemRulesTable from '../components/admin/SystemRulesTable';
import PromptTemplatesTable from '../components/admin/PromptTemplatesTable';
import UserManagementTable from '../components/admin/UserManagementTable';
import RuleFormModal from '../components/admin/RuleFormModal';
import PromptFormModal from '../components/admin/PromptFormModal';
import RuleUploadModal from '../components/admin/RuleUploadModal';
import ViewItemModal from '../components/admin/ViewItemModal';

const DIAGRAM_TYPES = [
  'Sequence Diagram',
  'Class Diagram',
  'ER Diagram',
  'Flowchart',
  'Use Case Diagram',
  'Consistency Check',
  'Guidelines',
  'Global',
];

export default function AdminPage() {
  // Current Tab: 'rules' | 'prompts' | 'users'
  const [activeTab, setActiveTab] = useState('rules');

  // Loading & Alert State
  const [loading, setLoading] = useState(false);
  const [actionLoading, setActionLoading] = useState(false);
  const [notification, setNotification] = useState(null);

  // Data States
  const [rules, setRules] = useState([]);
  const [prompts, setPrompts] = useState([]);
  const [users, setUsers] = useState([]);

  // Search & Filter
  const [searchTerm, setSearchTerm] = useState('');
  const [typeFilter, setTypeFilter] = useState('ALL');

  // Modals States
  const [isRuleModalOpen, setIsRuleModalOpen] = useState(false);
  const [editingRule, setEditingRule] = useState(null);
  const [ruleFormData, setRuleFormData] = useState({
    ruleName: '',
    diagramType: 'Sequence Diagram',
    regexOrCondition: '',
    isActive: true,
  });

  const [isPromptModalOpen, setIsPromptModalOpen] = useState(false);
  const [editingPrompt, setEditingPrompt] = useState(null);
  const [promptFormData, setPromptFormData] = useState({
    name: '',
    diagramType: 'Sequence Diagram',
    content: '',
  });

  const [isViewModalOpen, setIsViewModalOpen] = useState(false);
  const [viewingItem, setViewingItem] = useState(null);

  const [isUploadModalOpen, setIsUploadModalOpen] = useState(false);
  const [uploadFile, setUploadFile] = useState(null);
  const [uploadDiagramType, setUploadDiagramType] = useState('Sequence Diagram');

  // Toast Notification
  const showNotification = (message, type = 'success') => {
    setNotification({ message, type });
    setTimeout(() => setNotification(null), 4000);
  };

  // Fetch Data based on active tab
  const fetchData = async () => {
    setLoading(true);
    try {
      if (activeTab === 'rules') {
        const res = await adminService.getSystemRules();
        const data = res?.data || res || [];
        setRules(Array.isArray(data) ? data : []);
      } else if (activeTab === 'prompts') {
        const res = await adminService.getPrompts();
        const data = res?.data || res || [];
        setPrompts(Array.isArray(data) ? data : []);
      } else if (activeTab === 'users') {
        const res = await adminService.getUsers();
        const data = res?.data || res || [];
        setUsers(Array.isArray(data) ? data : []);
      }
    } catch (err) {
      console.error('Error loading admin data:', err);
      showNotification(err.response?.data?.message || err.message || 'Lỗi tải dữ liệu.', 'error');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
    setSearchTerm('');
    setTypeFilter('ALL');
  }, [activeTab]);

  // ==========================================
  // SYSTEM RULES HANDLERS
  // ==========================================
  const handleOpenRuleModal = (rule = null) => {
    if (rule) {
      setEditingRule(rule);
      setRuleFormData({
        ruleName: rule.ruleName,
        diagramType: rule.diagramType || 'Sequence Diagram',
        regexOrCondition: rule.regexOrCondition || '',
        isActive: rule.isActive ?? true,
      });
    } else {
      setEditingRule(null);
      setRuleFormData({
        ruleName: '',
        diagramType: 'Sequence Diagram',
        regexOrCondition: '',
        isActive: true,
      });
    }
    setIsRuleModalOpen(true);
  };

  const handleSaveRule = async (e) => {
    e.preventDefault();
    if (!ruleFormData.ruleName.trim()) {
      showNotification('Vui lòng nhập tên quy tắc', 'error');
      return;
    }

    try {
      setActionLoading(true);
      if (editingRule) {
        await adminService.updateSystemRule(editingRule.id, ruleFormData);
        showNotification('Cập nhật System Rule thành công!');
      } else {
        await adminService.createSystemRule(ruleFormData);
        showNotification('Tạo mới System Rule thành công!');
      }
      setIsRuleModalOpen(false);
      fetchData();
    } catch (err) {
      showNotification(err.response?.data?.message || 'Thao tác thất bại.', 'error');
    } finally {
      setActionLoading(false);
    }
  };

  const handleDeleteRule = async (id, name) => {
    if (!window.confirm(`Bạn có chắc chắn muốn xóa quy tắc "${name}"?`)) return;
    try {
      setActionLoading(true);
      await adminService.deleteSystemRule(id);
      showNotification('Đã xóa quy tắc thành công.');
      fetchData();
    } catch (err) {
      showNotification('Lỗi khi xóa quy tắc.', 'error');
    } finally {
      setActionLoading(false);
    }
  };

  const handleSeedRules = async () => {
    if (!window.confirm('Hệ thống sẽ quét và nạp các tài liệu từ thư mục RAG_Documents vào Database. Tiếp tục?')) return;
    try {
      setActionLoading(true);
      const res = await adminService.seedSystemRules(false);
      showNotification(res?.data?.message || res?.message || 'Đã nạp quy tắc RAG thành công!');
      fetchData();
    } catch (err) {
      showNotification('Lỗi nạp quy tắc từ thư mục.', 'error');
    } finally {
      setActionLoading(false);
    }
  };

  const handleClearAllRules = async () => {
    if (!window.confirm('CẢNH BÁO: Thao tác này sẽ XÓA TOÀN BỘ các System Rules và Embeddings trong hệ thống. Bạn có chắc chắn không?')) return;
    try {
      setActionLoading(true);
      await adminService.clearAllSystemRules();
      showNotification('Đã xóa toàn bộ quy tắc thành công.');
      fetchData();
    } catch (err) {
      showNotification('Lỗi xóa toàn bộ quy tắc.', 'error');
    } finally {
      setActionLoading(false);
    }
  };

  const handleUploadRuleFile = async (e) => {
    e.preventDefault();
    if (!uploadFile) {
      showNotification('Vui lòng chọn file (.md hoặc .txt)', 'error');
      return;
    }

    const formData = new FormData();
    formData.append('file', uploadFile);
    formData.append('diagramType', uploadDiagramType);

    try {
      setActionLoading(true);
      await adminService.uploadSystemRuleFile(formData);
      showNotification('Tải file quy tắc lên thành công!');
      setIsUploadModalOpen(false);
      setUploadFile(null);
      fetchData();
    } catch (err) {
      showNotification(err.response?.data?.message || 'Lỗi khi upload file.', 'error');
    } finally {
      setActionLoading(false);
    }
  };

  // ==========================================
  // PROMPT TEMPLATES HANDLERS
  // ==========================================
  const handleOpenPromptModal = (prompt = null) => {
    if (prompt) {
      setEditingPrompt(prompt);
      setPromptFormData({
        name: prompt.name,
        diagramType: prompt.diagramType || 'Sequence Diagram',
        content: prompt.content || '',
      });
    } else {
      setEditingPrompt(null);
      setPromptFormData({
        name: '',
        diagramType: 'Sequence Diagram',
        content: '',
      });
    }
    setIsPromptModalOpen(true);
  };

  const handleSavePrompt = async (e) => {
    e.preventDefault();
    if (!promptFormData.name.trim() || !promptFormData.content.trim()) {
      showNotification('Vui lòng nhập đầy đủ Tên và Nội dung Prompt', 'error');
      return;
    }

    try {
      setActionLoading(true);
      if (editingPrompt) {
        await adminService.updatePrompt(editingPrompt.id, promptFormData);
        showNotification('Cập nhật Prompt Template thành công!');
      } else {
        await adminService.createPrompt(promptFormData);
        showNotification('Tạo mới Prompt Template thành công!');
      }
      setIsPromptModalOpen(false);
      fetchData();
    } catch (err) {
      showNotification(err.response?.data?.message || 'Thao tác thất bại.', 'error');
    } finally {
      setActionLoading(false);
    }
  };

  const handleDeletePrompt = async (id, name) => {
    if (!window.confirm(`Bạn có chắc muốn xóa Prompt Template "${name}"?`)) return;
    try {
      setActionLoading(true);
      await adminService.deletePrompt(id);
      showNotification('Đã xóa Prompt Template thành công.');
      fetchData();
    } catch (err) {
      showNotification('Lỗi khi xóa Prompt Template.', 'error');
    } finally {
      setActionLoading(false);
    }
  };

  // ==========================================
  // USER MANAGEMENT HANDLERS
  // ==========================================
  const handleToggleUserRole = async (user) => {
    const newRole = user.role === 'Admin' ? 'User' : 'Admin';
    if (!window.confirm(`Bạn có muốn đổi quyền của "${user.email}" sang ${newRole}?`)) return;

    try {
      setActionLoading(true);
      await adminService.updateUserRole(user.id, newRole);
      showNotification(`Đã cập nhật quyền của ${user.email} thành ${newRole}!`);
      fetchData();
    } catch (err) {
      showNotification(err.response?.data?.message || 'Lỗi cập nhật quyền.', 'error');
    } finally {
      setActionLoading(false);
    }
  };

  const handleDeleteUser = async (id, email) => {
    if (!window.confirm(`CẢNH BÁO: Xóa người dùng "${email}" vĩnh viễn khỏi hệ thống?`)) return;

    try {
      setActionLoading(true);
      await adminService.deleteUser(id);
      showNotification('Đã xóa tài khoản người dùng thành công.');
      fetchData();
    } catch (err) {
      showNotification(err.response?.data?.message || 'Lỗi xóa người dùng.', 'error');
    } finally {
      setActionLoading(false);
    }
  };

  // Filtered lists
  const filteredRules = rules.filter((r) => {
    const matchName = (r.ruleName || '').toLowerCase().includes(searchTerm.toLowerCase()) ||
                      (r.diagramType || '').toLowerCase().includes(searchTerm.toLowerCase());
    const matchType = typeFilter === 'ALL' || (r.diagramType || '').toLowerCase() === typeFilter.toLowerCase();
    return matchName && matchType;
  });

  const filteredPrompts = prompts.filter((p) => {
    const matchName = (p.name || '').toLowerCase().includes(searchTerm.toLowerCase()) ||
                      (p.diagramType || '').toLowerCase().includes(searchTerm.toLowerCase());
    const matchType = typeFilter === 'ALL' || (p.diagramType || '').toLowerCase() === typeFilter.toLowerCase();
    return matchName && matchType;
  });

  const filteredUsers = users.filter((u) => {
    const matchEmail = (u.email || '').toLowerCase().includes(searchTerm.toLowerCase()) ||
                       (u.userName || '').toLowerCase().includes(searchTerm.toLowerCase());
    const matchRole = typeFilter === 'ALL' || (u.role || '').toLowerCase() === typeFilter.toLowerCase();
    return matchEmail && matchRole;
  });

  return (
    <div style={{ padding: '24px 32px', maxWidth: '1400px', margin: '0 auto' }}>
      {/* Toast Notification */}
      {notification && (
        <div
          style={{
            position: 'fixed',
            top: '24px',
            right: '24px',
            zIndex: 9999,
            display: 'flex',
            alignItems: 'center',
            gap: '10px',
            padding: '12px 18px',
            borderRadius: '8px',
            backgroundColor: notification.type === 'error' ? '#fef2f2' : '#f0fdf4',
            color: notification.type === 'error' ? '#b91c1c' : '#15803d',
            border: `1px solid ${notification.type === 'error' ? '#fecaca' : '#bbf7d0'}`,
            boxShadow: '0 10px 15px -3px rgba(0, 0, 0, 0.1)',
            fontSize: '13.5px',
            fontWeight: 500,
          }}
        >
          {notification.type === 'error' ? <AlertCircle size={18} /> : <CheckCircle2 size={18} />}
          <span>{notification.message}</span>
        </div>
      )}

      {/* Header */}
      <AdminHeader
        onRefresh={fetchData}
        loading={loading}
        actionLoading={actionLoading}
      />

      {/* Navigation Tabs */}
      <AdminTabs
        activeTab={activeTab}
        onTabChange={setActiveTab}
        rulesCount={rules.length}
        promptsCount={prompts.length}
        usersCount={users.length}
      />

      {/* Action Bar (Search, Filters, Action Buttons) */}
      <AdminActionBar
        activeTab={activeTab}
        searchTerm={searchTerm}
        onSearchChange={setSearchTerm}
        typeFilter={typeFilter}
        onTypeFilterChange={setTypeFilter}
        diagramTypes={DIAGRAM_TYPES}
        onOpenRuleModal={handleOpenRuleModal}
        onOpenUploadModal={() => setIsUploadModalOpen(true)}
        onSeedRules={handleSeedRules}
        onClearAllRules={handleClearAllRules}
        onOpenPromptModal={handleOpenPromptModal}
        actionLoading={actionLoading}
      />

      {/* Tab 1: System Rules Table */}
      {activeTab === 'rules' && (
        <SystemRulesTable
          rules={filteredRules}
          loading={loading}
          onViewRule={(r) => {
            setViewingItem({
              title: r.ruleName,
              type: `System Rule (${r.diagramType})`,
              content: r.regexOrCondition || 'Không có nội dung',
            });
            setIsViewModalOpen(true);
          }}
          onEditRule={handleOpenRuleModal}
          onDeleteRule={handleDeleteRule}
        />
      )}

      {/* Tab 2: Prompt Templates Table */}
      {activeTab === 'prompts' && (
        <PromptTemplatesTable
          prompts={filteredPrompts}
          loading={loading}
          onViewPrompt={(p) => {
            setViewingItem({
              title: p.name,
              type: `Prompt Template (${p.diagramType})`,
              content: p.content,
            });
            setIsViewModalOpen(true);
          }}
          onEditPrompt={handleOpenPromptModal}
          onDeletePrompt={handleDeletePrompt}
        />
      )}

      {/* Tab 3: User Management Table */}
      {activeTab === 'users' && (
        <UserManagementTable
          users={filteredUsers}
          loading={loading}
          actionLoading={actionLoading}
          onToggleUserRole={handleToggleUserRole}
          onDeleteUser={handleDeleteUser}
        />
      )}

      {/* Modal: Rule Form */}
      <RuleFormModal
        isOpen={isRuleModalOpen}
        onClose={() => setIsRuleModalOpen(false)}
        editingRule={editingRule}
        formData={ruleFormData}
        onFormChange={setRuleFormData}
        onSubmit={handleSaveRule}
        actionLoading={actionLoading}
        diagramTypes={DIAGRAM_TYPES}
      />

      {/* Modal: Prompt Form */}
      <PromptFormModal
        isOpen={isPromptModalOpen}
        onClose={() => setIsPromptModalOpen(false)}
        editingPrompt={editingPrompt}
        formData={promptFormData}
        onFormChange={setPromptFormData}
        onSubmit={handleSavePrompt}
        actionLoading={actionLoading}
        diagramTypes={DIAGRAM_TYPES}
      />

      {/* Modal: Rule File Upload */}
      <RuleUploadModal
        isOpen={isUploadModalOpen}
        onClose={() => setIsUploadModalOpen(false)}
        uploadDiagramType={uploadDiagramType}
        onDiagramTypeChange={setUploadDiagramType}
        uploadFile={uploadFile}
        onFileChange={setUploadFile}
        onSubmit={handleUploadRuleFile}
        actionLoading={actionLoading}
        diagramTypes={DIAGRAM_TYPES}
      />

      {/* Modal: View Item Detail */}
      <ViewItemModal
        isOpen={isViewModalOpen}
        onClose={() => setIsViewModalOpen(false)}
        item={viewingItem}
      />
    </div>
  );
}
