import React, { useState, useEffect, useRef } from "react";
import Spinner from "../shared/Spinner";
import SearchRepositoryCard from "../shared/SearchRepositoryCard";
import { useAuth } from "../../context/AuthContext";
import { SearchRepos, GetSearchResults } from "../../ApiRequest/searchRepos";
import { SearchRepositoryItem } from "../../models/searchRepository";
import { getSearchLanguageName } from "../../services/features/searchOnRepos";
import CancelIcon from "@mui/icons-material/Cancel";
import { useSnackbar } from "notistack";
import { Varient } from "../../models/SnackbarMessageVarient";

export default function SearchOnRepos() {
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
    Value: "",
  });
  const [searchId, setSearchId] = useState<string | null>(null);
  const pollingInterval = useRef<NodeJS.Timeout | null>(null);

  useEffect(() => {
    if (searchId) {
      pollingInterval.current = setInterval(async () => {
        try {
          const response = await GetSearchResults(searchId);
          const data = response?.data?.data;

          if (data?.searchState === "Completed") {
            clearInterval(pollingInterval.current!);
			setSearchRepos([...searchRepos, ...data.reposResult]);
            setMoreReposToSearch(data.moreUserToSearch);
            setShowMoreButton(data.moreUserToSearch);
            enqueueSnackbar("Search Completed Successfully.", {
              autoHideDuration: 3000,
              anchorOrigin: { vertical: "top", horizontal: "center" },
              variant: Varient.SUCCESS,
            });
          }
        } catch (error) {
          console.error("Polling error:", error);
          clearInterval(pollingInterval.current!);
        }
      }, 1000);
    }

    return () => {
      if (pollingInterval.current) clearInterval(pollingInterval.current);
    };
  }, [searchId]);

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    handleApiCall(setLoading);
  }

  const handleApiCall = async (loadingCallback: any) => {
    if (!searchText || !searchHandle) {
      enqueueSnackbar("Please provide valid input.", {
        autoHideDuration: 2000,
        anchorOrigin: { vertical: "top", horizontal: "center" },
        variant: Varient.ERROR,
      });
      return;
    }

    try {
      const body = {
        GithubHandle: searchHandle,
        QueryText: searchText,
        QueryLanguage: searchLanguage.Value,
        QuerySearchStartIndex: querySearchStartIndex,
      };

      const config = {
        headers: {
          ...(isUserLoggedIn() && currentUser && { Authorization: `Bearer ${currentUser.AccessToken}` }),
        },
      };

      loadingCallback(true);
      const response = await SearchRepos(body, config);

      const searchIdResponse = response?.data?.data?.searchId;
      if (searchIdResponse) {
        setSearchId(searchIdResponse);
        enqueueSnackbar("Search started successfully.", {
          autoHideDuration: 3000,
          anchorOrigin: { vertical: "top", horizontal: "center" },
          variant: Varient.SUCCESS,
        });
      }

      loadingCallback(false);
    } catch (error) {
      console.error(error);
      enqueueSnackbar("Failed to initiate search.", {
        autoHideDuration: 3000,
        anchorOrigin: { vertical: "top", horizontal: "center" },
        variant: Varient.ERROR,
      });
      loadingCallback(false);
    }
  };

  const clear = () => {
    setQuerySearchStartIndex(0);
    setMoreReposToSearch(true);
    setSearchRepos([]);
    setShowMoreButton(false);
    setSearchId(null);
    if (pollingInterval.current) clearInterval(pollingInterval.current);
  };

  const handleSearch = (event: React.ChangeEvent<HTMLInputElement>) => {
    setQuerySearchStartIndex(0);
    setSearchHandle(event.target.value);
    clear();
  };

  const handleText = (event: React.ChangeEvent<HTMLInputElement>) => {
    setQuerySearchStartIndex(0);
    setSearchText(event.target.value);
    clear();
  };

  const toggleOpen = () => {
    setLanguageDropdownIsOpen(!languageDropdownIsOpen);
  };

  const handleLanguageOption = (event: any) => {
    setQuerySearchStartIndex(0);
    setSearchLanguage(languagesList[event.target.value]);
    clear();
  };

  return (
    <div className="flex flex-col justify-center items-center bg-zinc-300 pt-3 pb-28">
      <div className="p-14 rounded-lg shadow-lg bg-white">
        <div className="flex space-x-4">
          <button
            onClick={() => {
              setSearchOwnNetwork(true);
              setSearchHandle(getUserTokenDecodedInfo()?.UserName || "");
            }}
            className={`${searchOwnNetwork ? "bg-blue-600" : "bg-blue-400"} hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-full`}
          >
            Search On Your Network
          </button>
          <button
            onClick={() => {
              setSearchOwnNetwork(false);
              setSearchHandle("");
            }}
            className={`${!searchOwnNetwork ? "bg-blue-600" : "bg-blue-400"} hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-full`}
          >
            Search On Others' Network
          </button>
        </div>
        <form onSubmit={handleSubmit}>
          <div className="border-b border-teal-500 mt-6">
            <input
              className="appearance-none bg-transparent border-none w-full text-gray-700 mr-3 py-1 px-2 leading-tight focus:outline-none"
              type="text"
              placeholder="Enter Github Handle"
              onChange={handleSearch}
              value={searchHandle}
            />
          </div>
          <div className="border-b border-teal-500 mt-8 flex w-71">
            <input
              className="appearance-none bg-transparent border-none w-full text-gray-700 mr-3 py-1 px-2 leading-tight focus:outline-none"
              type="text"
              placeholder="Search text e.g., Solid Principle"
              onChange={handleText}
              value={searchText}
            />
          </div>
          <div className="text-center mt-12">
            <button
              disabled={loading}
              className="shadow bg-blue-500 hover:bg-blue-600 focus:shadow-outline focus:outline-none text-white font-bold py-2 px-4 rounded disabled:bg-blue-300"
              type="submit"
            >
              {loading ? <Spinner /> : "Search"}
            </button>
          </div>
        </form>
      </div>
      <div className="py-14 mt-8 rounded-lg shadow-lg bg-white w-61 flex flex-col justify-center items-center">
        <div className="text-2xl pb-10">Your Search Result</div>
        {searchRepos.map((item, index) => (
          <SearchRepositoryCard {...item} key={index} />
        ))}
        {isShowMoreButton && (
          <button
            disabled={moreButtonloading}
            onClick={() => handleApiCall(setMoreButtonloading)}
            className="cursor-pointer p-4 m-4 shadow bg-blue-500 hover:bg-blue-600 text-white font-bold py-2 px-4 rounded disabled:bg-blue-300 flex justify-center items-center disabled:cursor-wait"
            style={{ fontSize: "20px" }}
          >
            {moreButtonloading ? <Spinner /> : "+"}
          </button>
        )}
      </div>
    </div>
  );
}
