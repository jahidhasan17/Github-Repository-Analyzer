import { TimelineRepositoryItem } from "../../models/searchRepository";



export default function LeftTimeLineRepositoryCard(item: TimelineRepositoryItem) {
	return (
		<div className="my-20 flex justify-between flex-row-reverse items-center w-full left-timeline">
			<div className="order-1 w-39"></div>
			<div className="z-20 flex items-center order-1 bg-gray-800 shadow-xl w-timeline-button h-timeline-button rounded-full">
				<h1 className="mx-auto font-semibold text-lg text-white text-center">{item.CreatedDate}</h1>
			</div>
			{/* <div className="order-1 bg-red-400 rounded-lg shadow-xl w-5/12 px-6 py-4">
				<h3 className="mb-3 font-bold text-white text-xl">Lorem Ipsum</h3>
				<p className="text-sm font-medium leading-snug tracking-wide text-white text-opacity-100">Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.</p>
			</div> */}
			<div className="order-1 bg-red-500 rounded-lg shadow-xl w-39 px-6 py-3">
				<div className="flex flex-col justify-center items-center">
					<div className="flex flex-col justify-center items-center">
						<div className="font-bold text-white text-21 ">{item.Name}</div>
						{item.ForkName && (
							<div className="flex space-x-2 mb-1">
								<div className="text-white">
									Forked from
								</div>
								<div>
									<a className="text-blue-800" href={`https://github.com/${item.ForkName}`}>
										{item.ForkName}
									</a>
								</div>
							</div>
						)}
					</div>
					<div className="font-normal text-white text-18 text-center my-0">
						{item.Description}
					</div>
					<div className="flex my-1 text-white">
						<div className="flex flex-col justify-center font-normal text-xl">

							{item.Languages && item.Languages?.length > 0 && item.Languages?.map((item) => {
								return (
									<div className="py-1">{item.Name} {item.Parcentage}</div>
								)
							})}
							{item.Languages?.length == 0 && (
								<div>No Language Found</div>
							)}
						</div>
						<div className="border-l-2 ml-7 mr-10 "></div>
						<div className="">
							<div className="flex flex-col justify-center items-center font-semibold mb-2">
								<div className="text-22">{item.Star || 0}</div>
								<div className="text-5">Star</div>
							</div>
							<div className="flex flex-col justify-center items-center font-semibold mb-2">
								<div className="text-22">{item.Fork || 0}</div>
								<div className="text-5">Fork</div>
							</div>
						</div>
					</div>
					<div className="flex mt-2 text-base text-white">
						<div className="">Created at - {item.CreatedDate}</div>
						<div className="lg:mx-9 2xl:mx-20"></div>
						<div className="">Updated at - {item.ModifiedDate}</div>
					</div>
				</div>
			</div>
		</div>
	);
}