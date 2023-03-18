import React from 'react';
import { BrowserRouter as Router,Route, Routes } from "react-router-dom";
import { AuthProvider } from '../context/AuthContext';
import { GeneralRoute } from '../routes/GeneralRoute';
import { PublicRoute } from '../routes/PublicRoute';
import Home from './pages/Home';
import Login from './pages/Login';
import ReposTimeline from './pages/ReposTimeline';
import SearchOnRepos from './pages/SearchOnRepos';
import Signup from './pages/Signup';
import Layout from './shared/Layout';
import { SnackbarProvider } from 'notistack';

function App() {


  return (
	<Router>
		<SnackbarProvider hideIconVariant preventDuplicate>
			<AuthProvider>
				<Layout>
					<Routes>
						<Route path='/' element={<GeneralRoute component={Home} />} />
						<Route path='/search-on-repos' element={<GeneralRoute component={SearchOnRepos} />} />
						<Route path='/repos-timeline' element={<GeneralRoute component={ReposTimeline} />} />
						<Route 
							path='/login' 
							element={<PublicRoute component={Login} />}
						/>
						<Route 
							path='/signup' 
							element={<PublicRoute component={Signup} />}
						/>
					</Routes>
				</Layout>
			</AuthProvider>
		</SnackbarProvider>
	</Router>
  );
}

export default App;
