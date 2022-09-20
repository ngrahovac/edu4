import React from 'react';
import ReactDOM from 'react-dom/client';
import {
  BrowserRouter,
  Routes,
  Route
} from 'react-router-dom'
import TopNavbar from './nav/TopNavbar';
import Homepage from './pages/Homepage';
import Welcome from './pages/Welcome'

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <BrowserRouter>
      <Routes>
        { /* landing */}
        <Route path='/' element={<Welcome></Welcome>}></Route>

        {/* navbar layout route + top-level pages as children */}
        <Route element={<TopNavbar></TopNavbar>}>
          <Route path='/homepage' element={<Homepage></Homepage>}></Route>
        </Route>
      </Routes>
    </BrowserRouter>
  </React.StrictMode>
);