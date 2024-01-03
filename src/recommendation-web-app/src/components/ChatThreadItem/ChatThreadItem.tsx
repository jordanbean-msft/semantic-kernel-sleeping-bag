import Avatar from "@mui/material/Avatar";
import Box from "@mui/material/Box";
import Paper from "@mui/material/Paper";
import Typography from "@mui/material/Typography";
import PersonIcon from "@mui/icons-material/Person";
import SmartToyIcon from "@mui/icons-material/SmartToy";

interface ChatThreadItemProps {
    content: string;
    isUserRole: boolean;
}

export default function ChatThreadItem({ content, isUserRole }: ChatThreadItemProps) {
    return (
        <Paper elevation={12}>
            <Box sx={{
                display: "flex",
                p: 2,
                maxWidth: 1000
            }}>
                <Avatar>
                    {
                        isUserRole ? (
                            <PersonIcon />
                        ) : (
                            <SmartToyIcon />
                        )
                    }
                </Avatar>
                <Typography align="left" sx={{ p: 1 }}>{content}</Typography>
            </Box>
        </Paper>
    );
}