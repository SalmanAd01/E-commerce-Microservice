import express from 'express';
import { initializeModels, syncDatabase } from './models/index.js';

const app = express();
app.use(express.json());

app.get('/', (req, res) => {
    res.send('Hello, World ! here we goo');
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, async () => {
    try{
        console.log(`Server is running on port ${PORT}`);
        await initializeModels();
        await syncDatabase();
    } catch (error) {
        console.error("Error starting the server:", error);
    }
});

