import { Provider, Roles } from "../const/User";
import { User } from "./interfaces/User";
import sequelize from "../database/connection/sequelize";
import { DataTypes, Optional, Model } from "sequelize";
import { UserTokenModel } from "./UserTokenModel";

interface UserCreationAttributes extends Optional<User, "id"> {}

interface UserInstance extends Model<User, UserCreationAttributes>, User {
	createdAt?: Date;
	updatedAt?: Date;
}

const UserModel = sequelize.define<UserInstance>("User", {
	id: {
		type: DataTypes.UUID,
		autoIncrement: false,
		primaryKey: true,
		unique: true,
		allowNull: false,
		defaultValue: DataTypes.UUIDV4,
	},
	UserName: {
		type: DataTypes.STRING,
		allowNull: false,
	},
	Email: {
		type: DataTypes.STRING,
		allowNull: false,
	},
	Password: {
		type: DataTypes.STRING,
		allowNull: false,
	},
	Provider: {
		type: DataTypes.STRING,
		defaultValue: Provider.WEBSITE,
	},
	AvatarUrl: {
		type: DataTypes.STRING,
	},
	Roles: {
		type: DataTypes.ARRAY(DataTypes.STRING),
		defaultValue: [Roles.USER],
	}
});

UserModel.hasMany(UserTokenModel, {
	foreignKey: "UserId",
	sourceKey: "id",
	as: "UserTokens"
});

UserTokenModel.belongsTo(UserModel, {
	foreignKey: "UserId",
	as: "User"
});

export {UserModel};