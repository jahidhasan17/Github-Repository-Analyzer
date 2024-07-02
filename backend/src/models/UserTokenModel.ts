import { UserToken } from "./interfaces/UserToken";
import sequelize from "../database/connection/sequelize";
import { DataTypes, Optional, Model } from "sequelize";

interface UserCreationAttributes extends Optional<UserToken, "id"> {}

interface UserTokenInstance extends Model<UserToken, UserCreationAttributes>, UserToken {
	createdAt?: Date;
	updatedAt?: Date;
}

const UserTokenModel = sequelize.define<UserTokenInstance>("UserToken", {
	id: {
		type: DataTypes.UUID,
		autoIncrement: false,
		primaryKey: true,
		unique: true,
		allowNull: false,
		defaultValue: DataTypes.UUIDV4,
	},
	UserId: {
		type: DataTypes.UUID,
		allowNull: false,
		unique: true,
	},
	RefreshToken: {
		type: DataTypes.STRING,
		allowNull: false,
	},
	GithubAccessToken: {
		type: DataTypes.STRING,
	},
	RefreshTokenExpiryTime: {
		type: DataTypes.DATE,
		defaultValue: DataTypes.NOW,
		allowNull: false,
	}
});

export {UserTokenModel};