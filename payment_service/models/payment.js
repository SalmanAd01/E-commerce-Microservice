import { DataTypes, Model } from "sequelize";

/**
 * Payment model with Razorpay helpers.
 *
 * Environment variables required:
 * - RAZORPAY_KEY_ID
 * - RAZORPAY_KEY_SECRET
 */
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

    /**
     * Convenience: create a Payment DB record after successful Razorpay payment.
     * paymentData expected to include: orderId (local), paymentMethod, transactionId, amount, currency, status
     */
    static async createPaymentRecord(paymentData) {
        return Payment.create(paymentData);
    }
}