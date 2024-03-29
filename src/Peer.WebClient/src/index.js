import React from 'react';
import ReactDOM from 'react-dom/client';
import {
  BrowserRouter,
  Routes,
  Route
} from 'react-router-dom'
import TopNavbar from './comps/nav/TopNavbar';
import Welcome from './pages/Welcome';
import Signup from './pages/Signup';
import Landing from './pages/Landing'
import Publish from './pages/Publish';
import Discover from './pages/Discover';
import { Auth0Provider } from "@auth0/auth0-react"
import './index.css';
import Project from './pages/Project';
import EditProject from './pages/EditProject';
import Contributor from './pages/Contributor';
import Applications from './pages/Applications';
import Search from './pages/Search';
import MyContributions from './pages/MyContributions';
import NotFound from './pages/NotFound';


const root = ReactDOM.createRoot(document.getElementById('root'));
const domain = process.env.REACT_APP_EDU4_APP_DOMAIN;
const clientId = process.env.REACT_APP_EDU4_APP_CLIENT_ID;
const audience = process.env.REACT_APP_EDU4_API_IDENTIFIER;

root.render(
  <Auth0Provider
    domain={domain}
    clientId={clientId}
    redirectUri={window.location.origin}
    audience={audience}
    cacheLocation='localstorage'>
    <React.StrictMode>
      <BrowserRouter>
        <Routes>
          { /* pages without the top navbar */}
          <Route path='/' element={<Landing></Landing>}></Route>
          <Route path="/signup" element={<Signup></Signup>}></Route>

          {/* navbar layout route + top-level pages as children */}
          <Route element={<TopNavbar></TopNavbar>}>
            <Route path='/welcome' element={<Welcome></Welcome>}></Route>
            <Route path='/publish' element={<Publish></Publish>}></Route>
            <Route path='/discover' element={<Discover></Discover>}></Route>
            <Route path='/projects/:projectId' element={<Project></Project>}></Route>
            <Route path='/projects/:projectId/edit' element={<EditProject></EditProject>}></Route>
            <Route path='/contributors/:contributorId' element={<Contributor></Contributor>}></Route>
            <Route path='/applications/:selectedApplicationType' element={<Applications></Applications>}></Route>
            <Route path='/search' element={<Search></Search>}></Route>
            <Route path='/contributions' element={<MyContributions></MyContributions>}></Route>

            <Route path='/404' element={<NotFound></NotFound>}></Route>
            <Route path='/*' element={<NotFound></NotFound>}></Route>
          </Route>
        </Routes>
      </BrowserRouter>
    </React.StrictMode>
  </Auth0Provider>
);