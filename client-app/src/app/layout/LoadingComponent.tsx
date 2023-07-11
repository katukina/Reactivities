import React from 'react';
import {Dimmer, Loader} from "semantic-ui-react";

//Waiting cursor in case takes time load the data from API

interface Props {
    inverted?: boolean;
    content?: string;
}

export default function LoadingComponent({inverted = true, content = 'Loading...'}: Props) {
    return (
        <Dimmer active={true} inverted={inverted}>
            <Loader content={content} />
        </Dimmer>
    )
}