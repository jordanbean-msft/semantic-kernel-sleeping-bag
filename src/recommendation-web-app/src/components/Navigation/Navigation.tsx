import * as React from "react";
import AppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import StorefrontIcon from "@mui/icons-material/Storefront";
import ChatWindow from "../ChatWindow/ChatWindow";
import Stack from "@mui/material/Stack";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import RestartAlt from "@mui/icons-material/RestartAlt";
import { useState } from "react";

const pages = ["Chatbot"];
const settings = ["Profile", "Logout"];

function ResponsiveAppBar() {
    const [anchorElNav, setAnchorElNav] = useState(null);
    const [anchorElUser, setAnchorElUser] = useState(null);
    const [loggedIn, setLoggedIn] = useState(false);
    const [reset, setReset] = useState(false);

    const handleOpenNavMenu = (event: {
        currentTarget: React.SetStateAction<null>;
    }) => {
        setAnchorElNav(event.currentTarget);
    };
    const handleOpenUserMenu = (event: {
        currentTarget: React.SetStateAction<null>;
    }) => {
        setAnchorElUser(event.currentTarget);
    };

    const handleCloseNavMenu = () => {
        setAnchorElNav(null);
    };

    const handleCloseUserMenu = () => {
        setAnchorElUser(null);
    };

    return (
        <Stack spacing={2}>
        <Box>
            <AppBar position="static">
                <Container maxWidth={false}>
                    <Toolbar disableGutters>
                        <StorefrontIcon
                            sx={{ display: { xs: "none", md: "flex" }, mr: 1 }}
                        />
                        <Typography
                            variant="h5"
                            noWrap
                            component="a"
                            href="#app-bar-with-responsive-menu"
                            sx={{
                                mr: 3,
                                display: { xs: "none", md: "flex" },
                                fontFamily: "sans-serif",
                                fontWeight: 700,
                                color: "inherit",
                                textDecoration: "none",
                            }}
                        >
                            Contoso Retail Chatbot
                        </Typography>
                        <Typography variant="h6" noWrap sx={{ mr: 1 }}>
                            (with Azure OpenAI & Semantic Kernel)
                        </Typography>
                        <Box sx={{ flexGrow: 1 }} />
                        <Button color="inherit" onClick={() => setReset(true)} endIcon={<RestartAlt />}>Restart</Button>
                    </Toolbar>
                </Container>
                </AppBar>
            </Box>
            <Box padding={2}>
                <ChatWindow reset={reset} setReset={setReset} />
            </Box>
        </Stack>
    );
}
export default ResponsiveAppBar;
