import RazorpayProvider from './razorpayProvider.js';

export const PaymentProviderFactory = (name = 'razorpay') => {
  switch ((name || 'razorpay').toLowerCase()) {
    case 'razorpay':
      return new RazorpayProvider();
    default:
      throw new Error(`Unsupported payment provider: ${name}`);
  }
};

export default PaymentProviderFactory;
