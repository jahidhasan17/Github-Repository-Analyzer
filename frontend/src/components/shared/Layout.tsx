import Nav from "./Nav";
import LinkedInIcon from '@mui/icons-material/LinkedIn';
import GitHubIcon from '@mui/icons-material/GitHub';
import FacebookIcon from '@mui/icons-material/Facebook';

interface LayoutPros {
	children: JSX.Element
}

export default function Layout({ children }: LayoutPros) {
	console.log("Rendered Layout");
	return (
		<>
			<Nav />
			<main className={""}>
				<div className={""}>{children}</div>
			</main>
			<section className="bg-gray-700 w-full text-white" >
				<div className="2xl:pt-10 md:pt-20 pb-20 flex flex-col justify-center items-center">
					<div className="text-3xl">About Developer</div>
					<div className="flex space-x-2 pt-2">
						<a href="https://www.linkedin.com/in/jahid172/" rel="noreferrer" target="_blank" >
							<LinkedInIcon fontSize="large"/>
						</a>

						<a href="https://github.com/jahidhasan17" target="_blank" rel="noreferrer" >
							<GitHubIcon fontSize="large"/>
						</a>

						<a href="https://www.facebook.com/jahid.hasan1743/" target="_blank" rel="noreferrer">
							<FacebookIcon fontSize="large"/>
						</a>
						
					</div>
				</div>
			</section>
		</>
	);
}