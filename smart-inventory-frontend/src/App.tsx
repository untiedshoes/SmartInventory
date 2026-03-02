import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import Dashboard from './pages/Dashboard';
import ItemDetail from './pages/ItemDetail';

const App: React.FC = () => {
    return (
      <Router>
        <div style={{ padding: '20px' }}>
          <nav style={{ marginBottom: '20px' }}>
            <Link to="/" style={{ marginRight: '10px' }}>Dashboard</Link>
            <Link to="/item/1">Sample Item</Link>
          </nav>

          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/item/:id" element={<ItemDetail />} />
          </Routes>
        </div>
      </Router>
    );
};

export default App;