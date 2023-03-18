import { Link } from "react-router-dom";
import AccountCircleIcon from '@mui/icons-material/AccountCircle';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import { useState } from "react";
import Button from '@mui/material/Button';
import { useAuth } from "../../context/AuthContext";



export default function Account() {
	console.log("Rendered Account");


	const { logout, isAuthenticate, getUserTokenDecodedInfo, isUserLoggedIn } = useAuth();


	const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
	const open = Boolean(anchorEl);
	const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
	  setAnchorEl(event.currentTarget);
	};
	const handleClose = () => {
	  	setAnchorEl(null);
	};

	const logOut = () => {
		logout();
	}

	return (
		<div className="">
			{isUserLoggedIn() && isAuthenticate() && (
				<div className="flex">
					<div>
						<span className="text-2xl">{ getUserTokenDecodedInfo()?.UserName }</span>
					</div>
					<span className="m-0" title="Account">
						
						<Button
							id="basic-button"
							aria-controls={open ? 'basic-menu' : undefined}
							aria-haspopup="true"
							aria-expanded={open ? 'true' : undefined}
							onClick={handleClick}
						>
							<AccountCircleIcon />
						</Button>
							
						<Menu
							anchorEl={anchorEl}
							open={open}
							onClose={handleClose}
							MenuListProps={{
							'aria-labelledby': 'basic-button',
							}}
						>
							<MenuItem onClick={logOut}>Logout</MenuItem>
						</Menu>
					</span>
				</div>
			)} 
			{(!isUserLoggedIn() || !isAuthenticate()) && (
				<div>
					<Link to="/login">
						<button className="bg-transparent hover:bg-gray-500 text-blue-700 font-semibold hover:text-white py-2 px-4 border border-blue-500 hover:border-transparent rounded-full">
							Login
						</button>
					</Link>
					<Link to="/signup">
						<button className="bg-transparent hover:bg-gray-500 text-blue-700 font-semibold hover:text-white py-2 px-4 border border-blue-500 hover:border-transparent rounded-full ml-2">
							Signup
						</button>
					</Link>
				</div>
			)}
		</div>
	);
}