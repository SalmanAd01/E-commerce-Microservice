import { DataTypes, Model } from "sequelize";
import Razorpay from "razorpay";
import crypto from "crypto";

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
     * Create a Razorpay order.
     * amount should be provided in decimal (e.g. 12.34) and will be converted to paise (multiply by 100).
     * Returns the razorpay order object.
     */
    static async createRazorpayOrder({ amount, currency = "INR", receipt = undefined, payment_capture = 1 } = {}) {
        if (!process.env.RAZORPAY_KEY_ID || !process.env.RAZORPAY_KEY_SECRET) {
            throw new Error("RAZORPAY_KEY_ID and RAZORPAY_KEY_SECRET must be set in environment");
        }

        const razorpay = new Razorpay({
            key_id: process.env.RAZORPAY_KEY_ID,
            key_secret: process.env.RAZORPAY_KEY_SECRET,
        });

        const options = {
            amount: Math.round(Number(amount) * 100), // convert to smallest currency unit
            currency,
            receipt: receipt || `rcpt_${Date.now()}`,
            payment_capture,
        };

        return razorpay.orders.create(options);
    }

    /**
     * Verify a Razorpay payment signature.
     * For client-side flow, pass `razorpay_order_id`, `razorpay_payment_id`, `razorpay_signature`.
     * Returns boolean.
     */
    static verifyRazorpaySignature({ razorpay_order_id, razorpay_payment_id, razorpay_signature }) {
        if (!process.env.RAZORPAY_KEY_SECRET) {
            throw new Error("RAZORPAY_KEY_SECRET must be set in environment");
        }

        const generated_signature = crypto
            .createHmac("sha256", process.env.RAZORPAY_KEY_SECRET)
            .update(`${razorpay_order_id}|${razorpay_payment_id}`)
            .digest("hex");

        return generated_signature === razorpay_signature;
    }

    /**
     * Convenience: create a Payment DB record after successful Razorpay payment.
     * paymentData expected to include: orderId (local), paymentMethod, transactionId, amount, currency, status
     */
    static async createPaymentRecord(paymentData) {
        return Payment.create(paymentData);
    }
}