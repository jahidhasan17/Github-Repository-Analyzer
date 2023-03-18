import { useState } from "react";
import Spinner from "../shared/Spinner";
import SearchIcon from '@mui/icons-material/Search';
import SearchRepositoryCard from "../shared/SearchRepositoryCard";
import { useAuth } from "../../context/AuthContext";
import { SearchRepos } from "../../ApiRequest/searchRepos";
import { SearchRepositoryItem } from "../../models/searchRepository";
import { getSearchLanguageName } from "../../services/features/searchOnRepos";
import CancelIcon from '@mui/icons-material/Cancel';
import { useSnackbar } from 'notistack';
import { Varient } from "../../models/SnackbarMessageVarient";


export default function SearchOnRepos() {
	console.log("Rendered SearchOnRepos Component");

	const { isUserLoggedIn, currentUser, getUserTokenDecodedInfo } = useAuth();
	const { enqueueSnackbar } = useSnackbar();

	const languagesList = getSearchLanguageName();

	const [searchOwnNetwork, setSearchOwnNetwork] = useState<boolean>(false);
	const [loading, setLoading] = useState<boolean>(false);
	const [moreButtonloading, setMoreButtonloading] = useState<boolean>(false);
	const [searchText, setSearchText] = useState<string>("");
	const [searchHandle, setSearchHandle] = useState<string | undefined>("");
	const [searchRepos, setSearchRepos] = useState<SearchRepositoryItem[]>([]);
	const [isShowMoreButton, setShowMoreButton] = useState<boolean>(false);
	const [querySearchStartIndex, setQuerySearchStartIndex] = useState<number>(0);
	const [hasMoreReposToSearch, setMoreReposToSearch] = useState<boolean>(true);
	const [languageDropdownIsOpen, setLanguageDropdownIsOpen] = useState<boolean>(false);
	const [searchLanguage, setSearchLanguage] = useState<any>({
		Name: "Language",
		Value: ""
	});


	async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
		event.preventDefault();
		console.log("Called handleSubmit");
		handleApiCall(setLoading);
	}

	const handleSearchRepos = () => {
		handleApiCall(setMoreButtonloading);
	}

	const handleApiCall = (loadingCallback: any) => {

		if(hasMoreReposToSearch === false) {
			
			enqueueSnackbar("Search Finished", {
				autoHideDuration: 2000,
				anchorOrigin: {
					vertical: 'top',
					horizontal: 'center'
				},
				variant: "error"
			})
			return;
		}

		try{

			let body = {
				GithubHandle: searchHandle,
				QueryText : searchText,
				QueryLanguage : searchLanguage.Value,
				QuerySearchStartIndex : querySearchStartIndex
			};

		
			console.log("searchRepos");
			console.log(searchRepos);

			const config = { 
				headers: {
					...(isUserLoggedIn() && currentUser !== null) && {Authorization: `Bearer ${currentUser.AccessToken}`}
				}
			};

			console.log(body, config);

			loadingCallback(true);

			SearchRepos(body, config).then((data) => {
				//console.log("Data is");
				console.log(data);

				if(!data?.data?.data?.ReposResult?.length || data?.data?.data?.ReposResult?.length === 0) {
					enqueueSnackbar("Opps!! No Repository Found. You may Search Again or Click More button", {
						autoHideDuration: 4000,
						anchorOrigin: {
							vertical: 'top',
							horizontal: 'center'
						},
						variant: Varient.ERROR
					});
				}else{
					enqueueSnackbar(`${data?.data?.data?.ReposResult?.length} Repositories Found.`, {
						autoHideDuration: 3000,
						anchorOrigin: {
							vertical: 'top',
							horizontal: 'center'
						},
						variant: Varient.SUCCESS
					});
				}

				if(data?.data?.data?.ReposResult) {
					data?.data?.data?.ReposResult.sort((item1: any, item2: any) => {
						return item1?.Star > item2.Star;
					})
				}
				setSearchRepos([...searchRepos, ...data?.data?.data?.ReposResult]);
				setShowMoreButton(data?.data?.data?.HasMoreUserToSearch);
				setMoreReposToSearch(data?.data?.data?.HasMoreUserToSearch)
				if(data?.data?.data?.UserSearchDoneCount) setQuerySearchStartIndex(data?.data?.data?.UserSearchDoneCount);
				//console.log(searchRepos);
				loadingCallback(false);
			}).catch((error) => {
				console.log(error);
				loadingCallback(false);
			});

		}catch(error) {
			console.log(error);
			loadingCallback(false);
		}
	}

	const clear = () => {
		setQuerySearchStartIndex(0);
		setMoreReposToSearch(true);
		setSearchRepos([]);
		setShowMoreButton(false);
	}

	const isValidForOwnNetworkSearch = () => {
		if(isUserLoggedIn() && getUserTokenDecodedInfo()?.UserName) {
			setSearchOwnNetwork(true);
			//const userName: string = getUserTokenDecodedInfo() !== null ? getUserTokenDecodedInfo()?.UserName : "";
			setSearchHandle(getUserTokenDecodedInfo()?.UserName);
		}else{
			enqueueSnackbar("Please Login First!", {
				autoHideDuration: 2000,
				anchorOrigin: {
					vertical: 'top',
					horizontal: 'center'
				},
				variant: Varient.WARNING
			});
		}
	}

	const handleSearch = (event: React.ChangeEvent<HTMLInputElement>) => {
		setQuerySearchStartIndex(0);
		setSearchHandle(event.target.value);
		clear();
	}

	const handleText = (event: React.ChangeEvent<HTMLInputElement>) => {
		setQuerySearchStartIndex(0);
		setSearchText(event.target.value);
		clear();
	}

	const handleSearchOtherNetwork = () => {
		setSearchOwnNetwork(false);
		setSearchHandle("");
	}

	const toggleOpen = () => {
		setLanguageDropdownIsOpen(!languageDropdownIsOpen);
	}

	const handleLanguageOption = (event: any) => {
		console.log(event.target.value);
		setQuerySearchStartIndex(0);
		setSearchLanguage(languagesList[event.target.value]);
		clear();
	}


	return (
		<div className="flex flex-col justify-center items-center bg-zinc-300 pt-3 pb-28">
			<div className="p-14 rounded-lg shadow-lg bg-white ">

				<div className="flex space-x-4">
					<button onClick={isValidForOwnNetworkSearch} className={`${searchOwnNetwork ? "bg-blue-600" : "bg-blue-400"} hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-full`}>
						Search On Your Network
					</button>
					
					<button onClick={handleSearchOtherNetwork} className={`${!searchOwnNetwork ? "bg-blue-600" : "bg-blue-400"} hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-full`}>
						Search On Others Network
					</button>
				</div>

				<form onSubmit={handleSubmit}>
		

					{searchOwnNetwork && <div className="border-b border-teal-500 mt-6">
						<input disabled value={getUserTokenDecodedInfo()?.UserName} className="appearance-none bg-transparent border-none w-full text-gray-700 mr-3 py-2 px-2 leading-tight focus:outline-none disabled:bg-slate-200 disabled:text-slate-500 disabled:border-slate-500 disabled:shadow-none
		invalid:border-pink-500" type="text" placeholder="Enter Github Handle"></input>
					</div>}

					{!searchOwnNetwork && <div className="border-b border-teal-500 mt-6">
						<input className="appearance-none bg-transparent border-none w-full text-gray-700 mr-3 py-1 px-2 leading-tight focus:outline-none" type="text" placeholder="Enter Github Handle" onChange={handleSearch} value={searchHandle}></input>
					</div>}

					<div className="flex">
						<div className="border-b border-teal-500 mt-8 flex w-71">
							<input className="appearance-none bg-transparent border-none w-full text-gray-700 mr-3 py-1 px-2 leading-tight focus:outline-none" type="text" placeholder="Search text e.g. Solid Principle" onChange={handleText} value={searchText}></input>
								{/* <SearchIcon sx={{ color: "black" }}/> */}
						</div>

						<div onClick={toggleOpen}>
							<button id="dropdownDefault" data-toggle="dropdown" className="text-slate-600 bg-slate-200 hover:bg-slate-300 font-medium rounded-lg text-sm px-4 py-2 mt-6 ml-5 text-center inline-flex items-center" type="button">
								<div className="mr-1">
									{searchLanguage.Name} 
								</div>
								{searchLanguage.Name === "Language" && (
									<svg className="ml-2 w-4 h-4" aria-hidden="true" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 9l-7 7-7-7"></path>
									</svg>
								)}
								{searchLanguage.Name !== "Language" && languageDropdownIsOpen === false && <CancelIcon/>}
							</button>
							<div id="dropdown" className={`${languageDropdownIsOpen ? "" : "hidden"} z-10 w-44 bg-white rounded divide-y divide-gray-100 shadow absolute `}>
								<ul className="py-1 text-sm text-gray-700"  aria-labelledby="dropdownDefault">
									{languagesList.map((item, index) => {
										return (
											<li key={index}>
												<button type="button" onClick={handleLanguageOption} value={index} className="block py-2 px-4 hover:bg-gray-100 w-full text-left">{item.Name}</button>
											</li>
										)
									})}
									
								</ul>
							</div>
						</div>
					</div>


					<div className="text-center mt-12">
						<button disabled={loading} className="shadow bg-blue-500 hover:bg-blue-600 focus:shadow-outline focus:outline-none text-white font-bold py-2 px-4 rounded disabled:bg-blue-300" type="submit">
							{loading && <Spinner/>}
							{loading && "Searching..."}
							{!loading && "Search"}
						</button>
					</div>
				</form>
			</div>

			<div className="py-14 mt-8 rounded-lg shadow-lg bg-white w-61 flex flex-col justify-center items-center">
				<div className="text-2xl pb-10">
					Your Search Result
				</div>

				{searchRepos.map((item, index) => {
					return <SearchRepositoryCard {...item}></SearchRepositoryCard>
				})}

				{isShowMoreButton === true && (
					<button disabled={moreButtonloading} onClick={handleSearchRepos} className=" cursor-pointer p-4 m-4 2xl:w-51 xl:w-61 lg:w-71 shadow bg-blue-500 hover:bg-blue-600 focus:shadow-outline focus:outline-none text-white font-bold py-2 px-4 rounded disabled:bg-blue-300 flex justify-center items-center disabled:cursor-wait" style={{ fontSize : "20px" }}>
						{moreButtonloading && <Spinner/>}
						{moreButtonloading && "Searching..."}
						{!moreButtonloading && "+"}
					</button>
				)}

			</div>
		</div>
	);
}