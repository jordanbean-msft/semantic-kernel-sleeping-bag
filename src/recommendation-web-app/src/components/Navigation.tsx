import * as React from "react";
import AppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import StorefrontIcon from "@mui/icons-material/Storefront";
import Request from "./Request/Request";

const pages = ["Chatbot"];
const settings = ["Profile", "Logout"];

function ResponsiveAppBar() {
    const [anchorElNav, setAnchorElNav] = React.useState(null);
    const [anchorElUser, setAnchorElUser] = React.useState(null);
    const [loggedIn, setLoggedIn] = React.useState(false);

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
        <Container maxWidth={false}>
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
                        <Typography variant="h6" noWrap>
                            (with Azure OpenAI & Semantic Kernel)
                        </Typography>
                    </Toolbar>
                </Container>
            </AppBar>
            <Request />
        </Container>
    );
}
export default ResponsiveAppBar;
