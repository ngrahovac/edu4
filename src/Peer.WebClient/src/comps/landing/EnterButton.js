import React from 'react'
import { useAuth0 } from '@auth0/auth0-react'

const EnterButton = () => {
    const { loginWithRedirect } = useAuth0();

    return (
        <button
            type="button"
            className='px-3 py-2 rounded-md bg-blue-500 hover:bg-blue-700 text-stone-50 font-semibold text-md ml-2'
            onClick={() => loginWithRedirect()}>
            Enter
        </button>
    )
}

export default EnterButton