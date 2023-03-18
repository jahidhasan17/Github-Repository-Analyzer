import { Link } from "react-router-dom";
import logo from "../../assets/images/logo.jpg";
import Account from "./Account";
import { useAuth } from "../../context/AuthContext";

   

export default function Nav() {

	console.log("Rendered Nav");

	const { } = useAuth();

	return (
	<nav className="mx-auto px-6 pt-6 pb-3 bg-slate-100">
		<div className="flex items-center justify-between">
			<Link to="/" className="flex items-center">
				<img className="w-16 ml-5 rounded" src={logo} alt="logo images" />
				<h3 className="ml-3 text-2xl border-slate-300 border-l-2 pl-2 hidden md:block">Ultimate Github Repository Analyzer</h3>
			</Link>
			<div className="flex space-x-3 2xl:mr-300 lg:mr-50">
				<Link to="/search-on-repos">
					<button className={`text-xl border-r-2 pr-4 border-slate-500 hover:underline underline-offset-16 ${window.location.href.includes("search-on-repos") ? "underline" : ""}`}>Search Repository</button>
				</Link>
				<Link to="/repos-timeline">
					<button className={`text-xl pr-3 pl-2 border-slate-500 hover:underline underline-offset-16 ${window.location.href.includes("repos-timeline") ? "underline" : ""}`}>Repository Timeline</button>
				</Link>
				{/* <Link to="/search-on-repos">
					<button className="text-xl pl-2 hover:underline underline-offset-16">Repository </button>
				</Link> */}
			</div>
			<Account />
		</div>
	</nav>
	);
}