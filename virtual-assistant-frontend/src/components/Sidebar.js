import React from 'react';
import { Drawer, List, ListItem, ListItemText } from '@mui/material';
import { Link } from 'react-router-dom';

const Sidebar = () => {
  return (
    <Drawer variant="permanent" anchor="right">
      <List>
        <ListItem button component={Link} to="/">
          <ListItemText primary="Home" />
        </ListItem>
        <ListItem button component={Link} to="/chat">
          <ListItemText primary="Chat" />
        </ListItem>
        <ListItem button component={Link} to="/tasks">
          <ListItemText primary="Tasks" />
        </ListItem>
      </List>
    </Drawer>
  );
};

export default Sidebar;
