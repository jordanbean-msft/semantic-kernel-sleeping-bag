import Avatar from "@mui/material/Avatar";
import Box from "@mui/material/Box";
import Grid from "@mui/material/Grid";
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
        //<Grid item xs={12}>
        //    <Grid container>
        //        {
        //            isUserRole ? (
        //                <div />
        //            ) : (
        //                <Grid item xs={6} />
        //            )
        //        }
        //        <Grid item xs={6}>
        //            <Paper elevation={12} >
        //                <Grid container spacing={2} wrap="nowrap">
        //                    <Grid item>
        //                        <Avatar>
        //                            {
        //                                avatar
        //                            }
        //                        </Avatar>
        //                    </Grid>
        //                    <Grid item xs>
        //                        {
        //                            item
        //                        }
        //                    </Grid>
        //                </Grid>
        //            </Paper>
        //        </Grid>
        //        {
        //            isUserRole ? (
        //                <Grid item xs={6} />
        //            ) : (
        //                <div />
        //            )
        //        }                </Grid>
        //</Grid>
    );
}