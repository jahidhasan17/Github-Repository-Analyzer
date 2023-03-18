import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { login } from "../../ApiRequest/auth";
import { useAuth } from "../../context/AuthContext";
import Spinner from "../shared/Spinner";

export default function Login() {

	console.log(process.env.REACT_APP_HOME_URL);

	const [loading, setLoading] = useState<boolean>(false);

	const { setCurrentUser } = useAuth();


	const [email, setEmail] = useState<string>();
	const [userName, setUserName] = useState<string>();
	const [password, setPassword] = useState<string>();
	const [error, setError] = useState<string>();

	let navigate = useNavigate();


	async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
		event.preventDefault();

		try{
			setError("");
			setLoading(true);

			login({
				"Email" : email,
				"UserName": userName,
				"Password" : password
			}).then((data) => {

				setCurrentUser({
					"AccessToken": data.data.AccessToken,
					"RefreshToken": data.data.RefreshToken,
				})

				navigate('/');
			}).catch((error) => {
				setLoading(false);
				setError(error.response.data.message);
			}).finally(() => {
				setLoading(false);
			})

		}catch(error) {
			console.log(error);
			setLoading(false);
		}

	}



	return (
		<div className="flex items-center justify-center 2xl:mb-20 md:mb-10">
			<div className="flex items-center justify-center flex-col border shadow-xl p-10 m-4 rounded-lg 2xl:mt-20 w-27">
				<div className="text-3xl font-bold mt-5">Login to Your account</div>
				<div className="mt-10 w-full">
					<form onSubmit={handleSubmit}>

						<div className="border-b border-teal-500">
							<input className="appearance-none bg-transparent border-none text-gray-700 leading-tight focus:outline-none" type="email" placeholder="Enter Email" onChange={(event: React.ChangeEvent<HTMLInputElement>) => setEmail(event.target.value)}></input>
						</div>

						<div className="border-b border-teal-500 mt-8">
							<input className="appearance-none bg-transparent border-none text-gray-700 leading-tight focus:outline-none" type="text" placeholder="Enter Username" onChange={(event: React.ChangeEvent<HTMLInputElement>) => setUserName(event.target.value)}></input>
						</div>

						<div className="border-b border-teal-500 mt-8">
							<input className="appearance-none bg-transparent border-none text-gray-700 leading-tight focus:outline-none" type="password" placeholder="Enter Password" onChange={(event: React.ChangeEvent<HTMLInputElement>) => setPassword(event.target.value)}></input>
						</div>

						<div className="text-center mt-10">
							<button disabled={loading} className="shadow bg-blue-600 hover:bg-blue-800 focus:shadow-outline focus:outline-none text-white font-bold py-2 px-4 rounded disabled:bg-blue-300 w-full" type="submit">
								{loading && <Spinner/>}
								Log in
							</button>
						</div>

						{error && <p className="py-2 px-4 rounded w-full bg-red-200 mt-3">{error}</p>}

						<div className="text-base text-center mt-1">
							Don't have an account? <Link to="/signup" className="text-blue-600">Signup</Link> instead
						</div>

						<div className="flex items-center my-4 before:flex-1 before:border-t before:border-gray-300 before:mt-0.5 after:flex-1 after:border-t after:border-gray-300 after:mt-0.5">
							<p className="text-center font-semibold mx-4 mb-0">OR</p>
						</div>

						<a href={`${process.env.REACT_APP_GITHUB_OAUTH_LOGIN_URL}${process.env.REACT_APP_HOME_URL}`} className="flex w-full uppercase rounded border shadow-lg p-2 items-center justify-center space-x-2">
							<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path d="M12 0c-6.626 0-12 5.373-12 12 0 5.302 3.438 9.8 8.207 11.387.599.111.793-.261.793-.577v-2.234c-3.338.726-4.033-1.416-4.033-1.416-.546-1.387-1.333-1.756-1.333-1.756-1.089-.745.083-.729.083-.729 1.205.084 1.839 1.237 1.839 1.237 1.07 1.834 2.807 1.304 3.492.997.107-.775.418-1.305.762-1.604-2.665-.305-5.467-1.334-5.467-5.931 0-1.311.469-2.381 1.236-3.221-.124-.303-.535-1.524.117-3.176 0 0 1.008-.322 3.301 1.23.957-.266 1.983-.399 3.003-.404 1.02.005 2.047.138 3.006.404 2.291-1.552 3.297-1.23 3.297-1.23.653 1.653.242 2.874.118 3.176.77.84 1.235 1.911 1.235 3.221 0 4.609-2.807 5.624-5.479 5.921.43.372.823 1.102.823 2.222v3.293c0 .319.192.694.801.576 4.765-1.589 8.199-6.086 8.199-11.386 0-6.627-5.373-12-12-12z"/></svg>
							<span>Continue with Github</span>
						</a>
					</form>
				</div>
			</div>
		</div>
	)
}