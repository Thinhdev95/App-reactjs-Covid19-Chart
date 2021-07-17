import React, { useEffect } from 'react';
import './App.css';
// import 'bootstrap/dist/css/bootstrap.min.css';
// import { BrowserRouter, Route, Link, Switch, Router } from 'react-router-dom';
import { BrowserRouter as Router,Route, Link, Switch } from 'react-router-dom';

import Chart_Summary from './components/Chart_Summary';
import ChartPie from './components/ChartPie';

function App(){
return (
  <div className="App">
      <Router>
          { /* Menu */}
          <div>
              <ul>
                  <li className="menu-item">
                      <Link to="/" style={{ textDecoration: 'none' }}>Trang chủ</Link>
                  </li>
                  <li className="menu-item">
                      <Link to="/sumary">Báo cáo tổng hợp</Link>
                  </li>
              </ul>
          </div>
          <hr />
          { /* Khai báo định tuyến*/}
          <div style={{ minHeight: 500, padding: 5 }}>
              <Switch>
                  <Route path="/sumary" component={Chart_Summary} />
                  <Route path="/" component={ChartPie} />
                  {/* <Route path="/">
                      <h2>TRANG CHỦ</h2>
                  </Route> */}
              </Switch>
          </div>
      </Router>
      <hr />
  </div>
)};

export default App;