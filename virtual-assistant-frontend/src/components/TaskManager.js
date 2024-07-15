import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Container, TextField, Button, List, ListItem, ListItemText } from '@mui/material';

const TaskManager = () => {
  const [tasks, setTasks] = useState([]);
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [location, setLocation] = useState('');

  useEffect(() => {
    const fetchTasks = async () => {
      const response = await axios.get('https://localhost:7255/api/Resources');
      setTasks(response.data);
    };
    fetchTasks();
  }, []);

  const addTask = async () => {
    const newTask = {
      name: name || '',
      description: description || '',
      location: location || ''
    };
    const response = await axios.post('https://localhost:7255/api/Resources', newTask);
    setTasks([...tasks, response.data]);
    setName('');
    setDescription('');
    setLocation('');
  };

  return (
    <Container>
      <h2>Task Manager</h2>
      <List>
        {tasks.map((task, index) => (
          <ListItem key={index}>
            <ListItemText primary={task.name} />
            <ListItemText primary={task.description} />
            <ListItemText primary={task.location} />
          </ListItem>
        ))}
      </List>
      <TextField
        label="Task Name"
        variant="outlined"
        fullWidth
        value={name}
        onChange={(e) => setName(e.target.value)}
      />
      <TextField
        label="Description"
        variant="outlined"
        fullWidth
        value={description}
        onChange={(e) => setDescription(e.target.value)}
        style={{ marginTop: '10px' }}
      />
      <TextField
        label="Location"
        variant="outlined"
        fullWidth
        value={location}
        onChange={(e) => setLocation(e.target.value)}
        style={{ marginTop: '10px' }}
      />
      <Button variant="contained" color="primary" onClick={addTask} style={{ marginTop: '10px' }}>
        Add Task
      </Button>
    </Container>
  );
};

export default TaskManager;
