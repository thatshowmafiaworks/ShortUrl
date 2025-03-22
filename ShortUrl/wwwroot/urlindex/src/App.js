import logo from './logo.svg';
import './App.css';
import { useState } from 'react';

function App() {


  return (
    <div className="App">
      {user.isAuthenticated &&
        <a href='/url/add' class="btn btn-primary">Add new</a>
      }
      <h1>All Shortened Urls</h1>
    </div>

  );
}

export default App;
