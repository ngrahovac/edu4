import React from 'react';
import ReactDOM from 'react-dom/client';
import Welcome from './pages/Welcome';
import {
  BrowserRouter,
  Routes,
  Route
} from 'react-router-dom'

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Welcome></Welcome>}></Route>
      </Routes>
    </BrowserRouter>
  </React.StrictMode>
);