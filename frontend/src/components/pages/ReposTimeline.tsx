import { useState } from "react";
import { SearchTimeline } from "../../ApiRequest/searchRepos";
import { useAuth } from "../../context/AuthContext";
import { TimelineRepositoryItem } from "../../models/searchRepository";
import { Varient } from "../../models/SnackbarMessageVarient";
import LeftTimeLineRepositoryCard from "../shared/LeftTimeLineRepositoryCard"
import RightTimeLineRepositoryCard from "../shared/RightTimeLineRepositoryCard"
import Spinner from "../shared/Spinner";
import { useSnackbar } from 'notistack';



export default function ReposTimeline() {


	const { isUserLoggedIn, currentUser, getUserTokenDecodedInfo } = useAuth();
	const { enqueueSnackbar } = useSnackbar();



	const [loading, setLoading] = useState<boolean>(false);
	const [firstUserHandle, setFirstUserHandle] = useState<string>("");
	const [secondUserHandle, setSecondUserHandle] = useState<string>("");
	const [firstUserSearchRepoStartIndex, setFirstUserSearchRepoStartIndex] = useState<number>(0);
	const [secondUserSearchRepoStartIndex, setSecondUserSearchRepoStartIndex] = useState<number>(0);
	const [reposTimeline, setReposTimeline] = useState<TimelineRepositoryItem[]>([]);
	const [hasMoreReposToSearch, setMoreReposToSearch] = useState<boolean>(true);
	const [isShowMoreButton, setShowMoreButton] = useState<boolean>(false);
	const [moreButtonloading, setMoreButtonloading] = useState<boolean>(false);


	const handleFristUser = (event: React.ChangeEvent<HTMLInputElement>) => {
		setFirstUserHandle(event.target.value);
		clear();
	}

	const handleSecondUser = (event: React.ChangeEvent<HTMLInputElement>) => {
		setSecondUserHandle(event.target.value);
		clear();
	}

	const clear = () => {
		setFirstUserSearchRepoStartIndex(0);
		setSecondUserSearchRepoStartIndex(0);
		setReposTimeline([]);
		setMoreReposToSearch(true);
		setShowMoreButton(false);
	}

	const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
		event.preventDefault();

		handleApiCall(setLoading);
	}

	const handleMoreRepos = () => {
		handleApiCall(setMoreButtonloading);
	}

	const handleApiCall = (loadingCallback: any) => {


		if(hasMoreReposToSearch === false) {
			enqueueSnackbar(`No more Repositories To Show`, {
				autoHideDuration: 6000,
				anchorOrigin: {
					vertical: 'top',
					horizontal: 'center'
				},
				variant: Varient.ERROR
			});
			return;
		}

		console.log(hasMoreReposToSearch);


		loadingCallback(true);

		try{
			let body = [
				{
					UserName: firstUserHandle,
        			StartFrom: firstUserSearchRepoStartIndex
				},
				{
					UserName: secondUserHandle,
        			StartFrom: secondUserSearchRepoStartIndex
				}
			];

			const config = { 
				headers: {
					...(isUserLoggedIn() && currentUser !== null) && {Authorization: `Bearer ${currentUser.AccessToken}`}
				}
			};

			console.log(body, config);
			SearchTimeline(body, config).then((data) => {
				console.log(data.data);
				setReposTimeline([...reposTimeline, ...data?.data?.data]);
				loadingCallback(false);
				setShowMoreButton(data?.data?.HasMoreReposForFirstUser || data?.data?.HasMoreReposForSecondUser);
				setMoreReposToSearch(data?.data?.HasMoreReposForFirstUser || data?.data?.HasMoreReposForSecondUser);
				
				if(data?.data?.TotalFirstUserRepoSearchDone) {
					setFirstUserSearchRepoStartIndex(data?.data?.TotalFirstUserRepoSearchDone);
				}

				if(data?.data?.TotalSecondUserRepoSearchDone) {
					setSecondUserSearchRepoStartIndex(data?.data?.TotalSecondUserRepoSearchDone);
				}


			
				enqueueSnackbar(`Found ${data?.data?.TotalFirstUserRepoFound} Repositories From First User and ${data?.data?.TotalSecondUserRepoFound} Repositories from Second User`, {
					autoHideDuration: 6000,
					anchorOrigin: {
						vertical: 'top',
						horizontal: 'center'
					},
					variant: Varient.SUCCESS
				});
			}).catch((error) => {
				console.log("Error Occured");
				console.log(error);
				loadingCallback(false);
			});

		}catch(error) {
			console.log(error);
			loadingCallback(false);
		}
	}

	return (
		<div className="">
			<div className="flex justify-center items-center">
				<div className="p-14 rounded-lg shadow-lg bg-white mb-10 2xl:w-35 lg:w-51">
					<form onSubmit={handleSubmit}>
						<div className="flex space-x-14 justify-center">
							<div className="w-42">
								<div className="border-b border-teal-500 mt-6">
									<input className="appearance-none bg-transparent border-none w-full text-gray-700 mr-3 py-1 px-2 leading-tight focus:outline-none" type="text" placeholder="Enter First User Github Handle" value={firstUserHandle} onChange={handleFristUser}></input>
								</div>
							</div>
							
							<div className="w-46">
								<div className="border-b border-teal-500 mt-6">
									<input className="appearance-none bg-transparent border-none w-full text-gray-700 mr-3 py-1 px-2 leading-tight focus:outline-none" type="text" placeholder="Enter Second User Github Handle" value={secondUserHandle} onChange={handleSecondUser}></input>
								</div>
							</div>
						</div>


						<div className="text-center mt-12">
							<button disabled={loading} className="shadow bg-blue-500 hover:bg-blue-600 focus:shadow-outline focus:outline-none text-white font-bold py-2 px-4 mr-5 rounded disabled:bg-blue-300" type="submit">
								{loading && <Spinner/>}
								{loading && "Searching..."}
								{!loading && "Search"}
							</button>
						</div>
					</form>
				</div>
			</div>
		
			<div className="2xl:mb-60 lg:mb-10 md:mb-10">
			<div className="container bg-gray-200 mx-auto w-full h-full">
				<div className="relative wrap overflow-hidden p-10 h-full">
					<div className="text-center text-4xl font-semibold font-serif mb-10">
						Timeline
					</div>
					{reposTimeline.length > 0 && (
						<div className="border-2-2 absolute border-opacity-20 border-gray-700 h-full border" style={{ left:"50%" }}></div>
					)}
					

					{reposTimeline.map((item, index) => {
						if(item.IsFristUsersRepos) {
							return <LeftTimeLineRepositoryCard {...item}></LeftTimeLineRepositoryCard>
						}else{
							return <RightTimeLineRepositoryCard {...item}></RightTimeLineRepositoryCard>
						}
						
					})}
		
				</div>
			</div>
			{isShowMoreButton === true && (
				<div className="flex justify-center items-center">
					<button disabled={moreButtonloading} onClick={handleMoreRepos} className=" cursor-pointer p-4 m-4 2xl:w-31 xl:w-41 lg:w-51 shadow bg-blue-500 hover:bg-blue-600 focus:shadow-outline focus:outline-none text-white font-bold py-2 px-4 rounded disabled:bg-blue-300 flex justify-center items-center disabled:cursor-wait" style={{ fontSize : "20px" }}>
						{moreButtonloading && <Spinner/>}
						{moreButtonloading && "Searching..."}
						{!moreButtonloading && "+"}
					</button>
				</div>
			)}
			</div>
		</div>
	)
}