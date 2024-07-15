import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Container, TextField, List, ListItem, ListItemText, Paper, Typography, Box, IconButton, Avatar } from '@mui/material';
import SendIcon from '@mui/icons-material/Send';

const Chatbot = () => {
  const [message, setMessage] = useState('');
  const [messages, setMessages] = useState([]);
  const [showChat, setShowChat] = useState(false);

  useEffect(() => {
    const welcomeMessage = {
      sender: 'bot',
      text: 'Salut je suis votre assistant virtuel! Que puis-je faire pour vous?',
      timestamp: new Date().toLocaleTimeString()
    };
    setMessages([welcomeMessage]);
  }, []);

  const sendMessage = async () => {
    if (message.trim() === '') return;

    const userMessage = {
      sender: 'user',
      text: message,
      timestamp: new Date().toLocaleTimeString()
    };
    setMessages(prevMessages => [...prevMessages, userMessage]);

    try {
      const response = await axios.post('https://localhost:7255/api/messages', { text: message }, {
        headers: {
          'Content-Type': 'application/json'
        }
      });

      const botMessage = {
        sender: 'bot',
        text: response.data.text,
        timestamp: new Date().toLocaleTimeString()
      };
      setMessages(prevMessages => [...prevMessages, botMessage]);
    } catch (error) {
      console.error("Erreur envoie de message: ", error);
      const errorMessage = {
        sender: 'bot',
        text: "Désolé, quelque chose s'est mal passé. Veuillez réessayer plus tard.",
        timestamp: new Date().toLocaleTimeString()
      };
      setMessages(prevMessages => [...prevMessages, errorMessage]);
    }

    setMessage('');
  };

  const formatMessage = (text) => {
    const urlRegex = /(https?:\/\/[^\s]+)/g;
    const parts = text.split(urlRegex);

    return parts.map((part, index) => {
      if (part.match(urlRegex)) {
        return <a key={index} href={part} target="_blank" rel="noopener noreferrer" style={{ color: 'blue', wordBreak: 'break-all' }}>{part}</a>;
      }
      return part;
    });
  };

  return (
    <div>
      <IconButton
        onClick={() => setShowChat(!showChat)}
        color="primary"
        style={{
          position: 'fixed',
          bottom: '30px',
          right: '20px',
          borderRadius: '50%',
          width: '50px',
          height: '50px',
          minWidth: '50px',
          minHeight: '50px',
          backgroundColor: '#103be6',
          color: 'white',
          padding: 0
        }}
      >
        <img
          src={`${process.env.PUBLIC_URL}/image.png`}
          alt="Logo"
          style={{
            width: '100%',
            height: '100%',
            borderRadius: '60%',
          }}
        />
      </IconButton>

      {showChat && (
        <Container style={{ position: 'fixed', bottom: '80px', right: '20px', maxWidth: '400px', zIndex: 1000 }}>
          <Box mb={0} display="flex" justifyContent="center" alignItems="center">
            <Typography variant="h6">Chatbot</Typography>
          </Box>
          <Paper style={{ padding: '10px', maxHeight: '400px', overflow: 'auto' }}>
            <List>
              {messages.map((msg, index) => (
                <ListItem 
                  key={index} 
                  style={{ 
                    justifyContent: msg.sender === 'user' ? 'flex-end' : 'flex-start',
                    marginBottom: '10px'
                  }}
                >
                  {msg.sender === 'bot' && <Avatar src={`${process.env.PUBLIC_URL}/favicon.png`} style={{ marginRight: '10px' }} />}
                  <div style={{ 
                    display: 'flex', 
                    flexDirection: 'column', 
                    alignItems: msg.sender === 'user' ? 'flex-end' : 'flex-start',
                    maxWidth: '70%'
                  }}>
                    <ListItemText
                      primary={
                        <Typography
                          variant="body1"
                          style={{
                            wordBreak: 'break-word',
                            whiteSpace: 'pre-wrap',
                            textAlign: msg.sender === 'user' ? 'right' : 'left',
                            display: 'inline-block'
                          }}
                        >
                          {formatMessage(msg.text)}
                        </Typography>
                      }
                      style={{
                        backgroundColor: msg.sender === 'user' ? '#c7f5be' : '#f1f8e9',
                        borderRadius: '10px',
                        padding: '10px',
                        display: 'inline-block'
                      }}
                    />
                    <Typography variant="caption" style={{ marginTop: '5px', color: '#888' }}>
                      {msg.timestamp}
                    </Typography>
                  </div>
                </ListItem>
              ))}
            </List>
          </Paper>
          <Box mt={2} display="flex">
            <TextField
              label="Message"
              variant="outlined"
              fullWidth
              value={message}
              onChange={(e) => setMessage(e.target.value)}
              onKeyPress={(e) => {
                if (e.key === 'Enter') sendMessage();
              }}
            />
            <IconButton color="primary" onClick={sendMessage} style={{ marginLeft: '10px' }}>
              <SendIcon />
            </IconButton>
          </Box>
        </Container>
      )}
    </div>
  );
};

export default Chatbot;