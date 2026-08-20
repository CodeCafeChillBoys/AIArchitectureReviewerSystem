import React from 'react';
import Sidebar from './Sidebar';
import Header from './Header';

export default function AppLayout({ children }) {
  return (
    <div className="app-container">
      <Sidebar />
      <div className="main-content">
        <Header />
        <main className="content-body">
          {children}
        </main>
      </div>
    </div>
  );
}
