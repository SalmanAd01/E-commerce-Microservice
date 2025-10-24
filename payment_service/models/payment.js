import { DataTypes, Model } from "sequelize";

export default class Payment extends Model {
    static initialize(sequelize) {
        Payment.init(
            {
                id: {
                    type: DataTypes.INTEGER,
                    primaryKey: true,
                    autoIncrement: true,
                },
                orderId: {
                    type: DataTypes.INTEGER,
                    allowNull: false,
                },
                paymentMethod: {
                    type: DataTypes.STRING,
                    allowNull: false,
                },
                transactionId: {
                    type: DataTypes.STRING,
                    allowNull: false,
                    unique: true,
                },
                amount: {
                    type: DataTypes.DECIMAL(10, 2),
                    allowNull: false,
                },
                currency: {
                    type: DataTypes.STRING,
                    allowNull: false,
                },
                status: {
                    type: DataTypes.ENUM("pending", "completed", "failed"),
                    allowNull: false,
                },
            },
            {
                sequelize,
                modelName: "Payment",
                tableName: "payments",
                timestamps: true,
            }
        );
    }
}