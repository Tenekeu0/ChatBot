import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { CssBaseline, Box } from '@mui/material';
import Header from './components/Header';
import Sidebar from './components/Sidebar';
import Home from './pages/Home';
import Tasks from './pages/Tasks';

const App = () => {
  return (
    <Router>
      <CssBaseline />
      <Header />
      <Box display="flex">
        {/* <Sidebar /> */}
        <Box component="main" flexGrow={1} p={3}>
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/tasks" element={<Tasks />} />
          </Routes>
        </Box>
      </Box>
    </Router>
  );
};

export default App;
