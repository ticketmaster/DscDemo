import React from 'react'
import { LinkContainer } from 'react-router-bootstrap'
import Header from './Header';
import BootstrapInformation from './BootstrapInformation';
import Button from 'react-bootstrap/lib/Button';

import PageHeader from 'react-bootstrap/lib/PageHeader';


const Main = (props) => (
  <div>
    <Header />
    <BootstrapInformation {...props} />
  </div>
)

export default Main