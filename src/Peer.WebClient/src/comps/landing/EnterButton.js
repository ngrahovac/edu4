import React from 'react'
import { useAuth0 } from '@auth0/auth0-react'
import PrimaryButton from '../buttons/PrimaryButton';

const EnterButton = () => {
    const { loginWithRedirect } = useAuth0();

    return (
        <PrimaryButton
            text="Enter"
            onClick={() => loginWithRedirect()}>
        </PrimaryButton>
    )
}

export default EnterButton