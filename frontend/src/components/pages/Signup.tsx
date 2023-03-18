import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { signup } from "../../ApiRequest/auth";
import Spinner from "../shared/Spinner";

export default function Signup() {


	const [loading, setLoading] = useState<boolean>(false);
	const [email, setEmail] = useState<string>();
	const [userName, setUserName] = useState<string>();
	const [password, setPassword] = useState<string>();
	const [confirmPassowrd, setConfirmPassowrd] = useState<string>();
	const [error, setError] = useState<string>();

	let navigate = useNavigate();


	async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
		event.preventDefault();

		if(password !== confirmPassowrd) {
			return setError("Password don't match");
		}

		try{
			setError("");
			setLoading(true);

			signup({
				"UserName" : userName,
				"Email" : email,
				"Password" : password
			}).then((data) => {
				navigate('/login');
			}).catch((error) => {
				console.log(error);
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
				<div className="text-3xl font-bold mt-5">Create an account</div>
				<div className="mt-10 w-full">
					<form onSubmit={handleSubmit}>
						<div className="border-b border-teal-500">
							<input className="appearance-none bg-transparent border-none text-gray-700 leading-tight focus:outline-none" type="email" placeholder="Enter Email" onChange={(event: React.ChangeEvent<HTMLInputElement>) => setEmail(event.target.value)}></input>
						</div>

						<div className="border-b border-teal-500">
							<input className="appearance-none bg-transparent border-none text-gray-700 leading-tight focus:outline-none mt-8" type="text" placeholder="Enter Username" onChange={(event: React.ChangeEvent<HTMLInputElement>) => setUserName(event.target.value)}></input>
						</div>

						<div className="border-b border-teal-500 mt-8">
							<input className="appearance-none bg-transparent border-none text-gray-700 leading-tight focus:outline-none" type="password" placeholder="Enter Password" onChange={(event: React.ChangeEvent<HTMLInputElement>) => setPassword(event.target.value)}></input>
						</div>

						<div className="border-b border-teal-500 mt-8">
							<input className="appearance-none bg-transparent border-none text-gray-700 leading-tight focus:outline-none" type="password" placeholder="Confirm Password" onChange={(event: React.ChangeEvent<HTMLInputElement>) => setConfirmPassowrd(event.target.value)}></input>
						</div>

						<div className="text-center mt-10">
							<button disabled={loading} className="shadow bg-blue-600 hover:bg-blue-800 focus:shadow-outline focus:outline-none text-white font-bold py-2 px-4 rounded disabled:bg-blue-300 w-full" type="submit">
								{loading && <Spinner/>}
								Create Account
							</button>
						</div>

						{error && <p className="py-2 px-4 rounded w-full bg-red-200 mt-3">{error}</p>}

						<div className="text-base text-center mt-1">
							Already have an account? <Link to="/login" className="text-blue-600">Login</Link> instead
						</div>
					</form>
				</div>
			</div>
		</div>
	)
}