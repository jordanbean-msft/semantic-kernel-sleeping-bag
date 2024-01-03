import Avatar from "@mui/material/Avatar";
import Box from "@mui/material/Box";
import Paper from "@mui/material/Paper";
import { SvgIconProps } from "@mui/material/SvgIcon";
import { ReactElement } from "react";

interface ChatThreadItemProps {
    avatar: ReactElement<SvgIconProps>;
    item: ReactElement;
    isUserRole: boolean;
}

export default function ChatThreadItem({ avatar, item, isUserRole }: ChatThreadItemProps) {
    return (
        <Paper elevation={12}>
            <Box sx={{
                display: "flex",
                p: 2,
                maxWidth: 1000
            }}>
                <Avatar>
                    {
                        avatar
                    }
                </Avatar>
                {
                    item
                }
            </Box>
        </Paper>
    );
}