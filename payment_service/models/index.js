import { Sequelize } from "sequelize";
import Payment from "./payment.js";

export const sequelize = new Sequelize(
    process.env.DATABASE_URL,
    {
        dialect: "postgres",
        logging: process.env.DEBUG ? console.log : false,
    }
);

export const initializeModels = () => {
    Payment.initialize(sequelize);
};

export const syncDatabase = async () => {
    try {
        await sequelize.authenticate();
        console.log("Database connection has been established successfully.");
        await sequelize.sync({ alter: true });
        console.log("All models were synchronized successfully.");
    } catch (error) {
        console.error("Unable to connect to the database:", error);
    }
};