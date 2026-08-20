import React from 'react';
import WorkspaceCard from './WorkspaceCard';
import EmptyWorkspaceState from './EmptyWorkspaceState';

export default function WorkspaceGrid({
  workspaces,
  searchTerm,
  onOpenCreateModal,
  onSelectWorkspace,
}) {
  if (workspaces.length === 0) {
    return (
      <EmptyWorkspaceState
        searchTerm={searchTerm}
        onOpenCreateModal={onOpenCreateModal}
      />
    );
  }

  return (
    <div style={{
      display: 'grid',
      gridTemplateColumns: 'repeat(auto-fill, minmax(290px, 1fr))',
      gap: '20px',
    }}>
      {workspaces.map((ws) => (
        <WorkspaceCard
          key={ws.id}
          workspace={ws}
          onClick={() => onSelectWorkspace(ws.id)}
        />
      ))}
    </div>
  );
}
