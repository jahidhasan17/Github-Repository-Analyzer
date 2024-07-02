import { Dialect, Sequelize } from 'sequelize';

const dbName = process.env.DB_NAME as string || "githubrepositoryanalyzer";
const dbUser = process.env.DB_USER as string || "me";
const dbHost = process.env.DB_HOST as string || "localhost";
const dbDriver = process.env.DB_DRIVER as Dialect || "postgres";
const dbPassword = process.env.DB_PASSWORD as string || "me";

console.log(dbName, dbUser, dbHost, dbDriver, dbPassword);

const sequelize = new Sequelize(dbName, dbUser, dbPassword, {
	host: dbHost,
	dialect: dbDriver,
  port: 5433,
  });
  
  export default sequelize;

// import mongoose, { mongo, connect } from "mongoose";

// const dbConnect = () => {
// 	try{
// 		const connectionParams = {
// 			useNewUrlParser: true,
// 			useUnifiedTopology: true
// 		};
	
// 		connect(`${process.env.MONGO_CONNECTION_STRING}`);
	
// 		mongoose.connection.on("error", (error) => {
// 			console.log("Error while connecting to database :" + error);
// 		});
	
// 		mongoose.connection.on("disconnected", () => {
// 			console.log("Mongodb connection disconnected");
// 		});
// 	}catch(error) {
// 		console.log("Error occure when database connection");
// 	}
// }

// export default dbConnect;