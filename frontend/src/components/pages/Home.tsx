import overviewImg from "../../assets/images/home-overview.jpg";
import searchFeaturesImg from "../../assets/images/search-features1.jpg";
import searchTimelineImg from "../../assets/images/search-timeline.jpg";
import { Link } from "react-router-dom";
import LinkedInIcon from '@mui/icons-material/LinkedIn';
import GitHubIcon from '@mui/icons-material/GitHub';
import FacebookIcon from '@mui/icons-material/Facebook';


function Home() {
	return (
		<div className="w-full">
			<div className="flex flex-col justify-center items-center">
				<section className="mb-12 w-90">
					<div className="flex py-24 items-center justify-around">
						<div className="w-2/5 flex flex-col items-center justify-center">
							<div className="text-4xl">
								A Simple Github Repository Analyser
							</div>
							<div className="py-5">
								<span>Lorem ipsum dolor sit, amet consectetur adipisicing elit. Eius recusandae laborum eum? Officiis fugiat facilis nemo dolorem cupiditate deleniti exercitationem voluptates error, eligendi reiciendis sequi eveniet totam sapiente. Ut, itaque!</span>
							</div>
						</div>
						<div className="w-2/5">
							<img className="rounded" src={overviewImg} alt="logo images" />
						</div>
					</div>
				</section>

				<section className="bg-slate-100 pt-7 flex flex-col justify-center items-center">
					<div className="text-center text-5xl">Features</div>
					<div className="w-90">
						<div className="flex py-24 items-center justify-around">
							<div className="w-2/5">
								<img className="rounded-lg" src={searchFeaturesImg} alt="logo images" />
							</div>
							<div className="w-2/5 flex flex-col items-center justify-center">
								<div className="text-4xl">
									Search Repository On User Network
								</div>
								<div className="py-5">
									<span>Lorem ipsum dolor sit, amet consectetur adipisicing elit. Eius recusandae laborum eum? Officiis fugiat facilis nemo dolorem cupiditate deleniti exercitationem voluptates error, eligendi reiciendis sequi eveniet totam sapiente. Ut, itaque!</span>
								</div>
								
								<Link to="/search-on-repos">
									<button className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-full">
										Get Started
									</button>
								</Link>

							</div>
						</div>
					</div>

					<div className="w-90">
						<div className="flex py-24 items-center justify-around">
							<div className="w-2/5 flex flex-col items-center justify-center">
								<div className="text-4xl">
								Generate Timeline Repository of Two User
								</div>
								<div className="py-5">
									<span>Lorem ipsum dolor sit, amet consectetur adipisicing elit. Eius recusandae laborum eum? Officiis fugiat facilis nemo dolorem cupiditate deleniti exercitationem voluptates error, eligendi reiciendis sequi eveniet totam sapiente. Ut, itaque!</span>
								</div>
								<Link to="/repos-timeline">
									<button className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-full">
										Get Started
									</button>
								</Link>
							</div>
							<div className="w-2/5">
								<img className="rounded-lg" src={searchTimelineImg} alt="logo images" />
							</div>
						</div>
					</div>
				</section>
			
			</div>
		</div>
	);
}

export default Home;