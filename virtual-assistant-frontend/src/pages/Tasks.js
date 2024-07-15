import React from 'react';
import { Container } from '@mui/material';
import TaskManager from '../components/TaskManager';
import Chat from './Chat';
const Tasks = () => {
  return (
    <Container>
      <TaskManager />
      <Chat/>
    </Container>
  );
};

export default Tasks;
