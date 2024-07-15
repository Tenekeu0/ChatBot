import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Container, Typography, List, ListItem, ListItemText, Paper } from '@mui/material';

const Resources = () => {
  const [resources, setResources] = useState([]);

  useEffect(() => {
    const fetchResources = async () => {
      try {
        const response = await axios.get('https://localhost:7255/api/resources');
        setResources(response.data);
      } catch (error) {
        console.error("Error fetching resources: ", error);
      }
    };

    fetchResources();
  }, []);

  return (
    <Container>
      <Typography variant="h4" mb={3}>Resources</Typography>
      <Paper style={{ padding: '20px', maxHeight: '500px', overflow: 'auto' }}>
        <List>
          {resources.map((resource) => (
            <ListItem key={resource.id}>
              <ListItemText
                primary={resource.name}
                secondary={resource.description}
                primaryTypographyProps={{ variant: 'body1' }}
              />
            </ListItem>
          ))}
        </List>
      </Paper>
    </Container>
  );
};

export default Resources;
