import React from 'react';
import ReactDOM from 'react-dom/client';
import {
  BrowserRouter,
  Routes,
  Route
} from 'react-router-dom'
import TopNavbar from './comps/nav/TopNavbar';
import Homepage from './pages/Homepage';
import Signup from './pages/Signup';
import Welcome from './pages/Welcome'
import Publish from './pages/Publish';
import { Auth0Provider } from "@auth0/auth0-react"
import './index.css';


const root = ReactDOM.createRoot(document.getElementById('root'));
const domain = process.env.REACT_APP_EDU4_APP_DOMAIN;
const clientId = process.env.REACT_APP_EDU4_APP_CLIENT_ID;
const audience = process.env.REACT_APP_EDU4_API_IDENTIFIER;

root.render(
  <Auth0Provider
    domain={domain}
    clientId={clientId}
    redirectUri={window.location.origin}
    audience={audience}>
    <React.StrictMode>
      <BrowserRouter>
        <Routes>
          { /* pages without the top navbar */}
          <Route path='/' element={<Welcome></Welcome>}></Route>
          <Route path="/signup" element={<Signup></Signup>}></Route>

          {/* navbar layout route + top-level pages as children */}
          <Route element={<TopNavbar></TopNavbar>}>
            <Route path='/homepage' element={<Homepage></Homepage>}></Route>
            <Route path='/publish' element={<Publish></Publish>}></Route>
          </Route>
        </Routes>
      </BrowserRouter>
    </React.StrictMode>
  </Auth0Provider>
);