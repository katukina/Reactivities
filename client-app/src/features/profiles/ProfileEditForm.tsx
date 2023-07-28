import { Form, Formik } from "formik";
import { observer } from "mobx-react-lite";
import { useStore } from "../../app/stores/store";
import * as Yup from 'yup';
import MyTextInput from "../../app/common/form/MyTextInput";
import { Button } from "semantic-ui-react";
import MyTextArea from "../../app/common/form/MyTextArea";

interface Props {
    setEditMode: (editMode: boolean) => void;
}

export default observer(function ProfileEditForm({ setEditMode }: Props) {
    const { profileStore: { profile, editProfile } } = useStore();

    const validationSchema = Yup.object().shape({
        displayName: Yup.string().required('The disaply name is required'),
      });

    return (
        <Formik
            initialValues={{displayName : profile?.displayName, bio: profile?.bio}}
            onSubmit={values => editProfile(values).then(() => {setEditMode(false);})
             }
            validationSchema={validationSchema} >
            {({ isSubmitting, isValid, dirty }) => (    
                <Form className='ui form'>
                    <MyTextInput placeholder='Display Name' name='displayName' />
                    <MyTextArea rows={3} placeholder='Bio' name='bio' />
                    <Button 
                        positive
                        type='submit'
                        content='Update profile'
                        loading={isSubmitting}
                        disabled={isSubmitting || !dirty || !isValid} 
                        floated='right' 
                        />
                </Form>
            )} 
        </Formik>
    )
})