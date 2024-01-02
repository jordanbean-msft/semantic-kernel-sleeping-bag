import Avatar from "@mui/material/Avatar";
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
        <Grid item xs={12}>
            <Grid container>
                {
                    isUserRole ? (
                        <div />
                    ) : (
                        <Grid item xs={6} />
                    )
                }
                <Grid item zeroMinWidth xs={6} >
                    <Paper>
                        <Grid container spacing={2} alignItems="center">
                            <Grid item xs={1}>
                                <Avatar>
                                    {
                                        avatar
                                    }
                                </Avatar>
                            </Grid>
                            <Grid item xs={11}>
                                {
                                    item
                                }
                            </Grid>
                        </Grid>
                    </Paper>
                </Grid>
                {
                    isUserRole ? (
                        <Grid item xs={6} />
                    ) : (
                        <div />
                    )
                }                </Grid>
        </Grid>
    );
}