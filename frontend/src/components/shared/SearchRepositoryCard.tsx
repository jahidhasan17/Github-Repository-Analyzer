import PersonOutlineIcon from '@mui/icons-material/PersonOutline';
import { SearchRepositoryItem } from '../../models/searchRepository';

export default function SearchRepositoryCard(item: SearchRepositoryItem) {
	return (
		<div className="p-4 m-4 2xl:w-51 xl:w-61 lg:w-71 bg-white rounded-lg border shadow-xl">
			<div className="text-xl flex justify-center">
				<PersonOutlineIcon sx={{ fontSize: 30 }}/>
				<a href={`https://github.com/${item.UserName}`} className="text-blue-700">
					{/* Hassan-Jahid17 */}
					{item.UserName}
				</a>
			</div>

			<div className="mt-5 ml-12">
				<div className="text-blue-700 text-2xl">
					<a href={`https://github.com/${item.UserName}/${item.Name}`}>
						{/* data-science-ipython-notebooks  */}
						{item.Name}
					</a>
				</div>
				{item.ForkName && (
					<div className="flex space-x-2 ">
						<div className="text-black-700">
							Forked from
						</div>
						<div>
							<a className="text-blue-700" href={`https://github.com/${item.ForkName}`}>
								{/* data-science-ipython-notebooks  */}
								{item.ForkName}
							</a>
						</div>
					</div>
				)}
			</div>

			<div className="flex ml-12 space-x-4 mt-5 font-semibold text-slate-700">
				{item.Language && (
					<div className="flex flex-col justify-center items-center border-r-2 border-slate-300 px-5">
						<div className="text-27">{item.Language}</div>
						<div className="text-5">Language</div>
					</div>
				)}
				<div className="flex flex-col justify-center items-center pl-5 pr-8 border-r-2 border-slate-300">
					<div className="text-27">{item.Star ? item.Star : 0}</div>
					<div className="text-5">Star</div>
				</div>
				<div className="flex flex-col justify-center items-center pl-5 pr-8">
					<div className="text-27">{item.Fork ? item.Fork : 0}</div>
					<div className="text-5">Fork</div>
				</div>
			</div>

			<div className="pt-5 text-xl ml-12">
				{item.Description}
			</div>

			<div className="text-center flex justify-center items-center space-x-2 text-lg pt-5">
				<div>
					Last modified
				</div>
				<div>
					{item.ModifiedDate}
				</div>
			</div>
		</div>
	);
}