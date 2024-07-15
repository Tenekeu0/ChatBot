import React from 'react';
import { Container, Typography } from '@mui/material';
import Chat from './Chat';

const Home = () => {
  return (
    <Container>
      <Typography variant="h4" gutterBottom>
        Bienvenue sur notre assistant virtual
      </Typography>
      <Typography variant="body1">
        Cet assistant vous aidera à chercher des informations et resources dans la companie.
      </Typography>
      <Chat/>
    </Container>
  );
};

export default Home;

